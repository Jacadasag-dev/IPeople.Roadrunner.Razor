using IPeople.Roadrunner.Razor;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Radzen;

namespace IPeople.Roadrunner.Razor.Components
{
    public partial class RrTable<T>
    {
        [Parameter]
        public T? Data { get; set; }

        [Parameter]
        public List<Models.RrPanelSetting> Settings { get; set; }

        private int pageSize;
        private bool dataChanged;
        private IList<IDictionary<string, object>> data;
        private IList<IDictionary<string, object>> filteredData;
        private IDictionary<string, Type> columns = new Dictionary<string, Type>();
        private Models.ColumnUserPreferences columnUserPreferences;
        private Models.PanelSettingsPreferences panelSettingsPreferences;
        private string tableType; // Change this for different table types

        protected override void OnAfterRender(bool firstRender)
        {
            base.OnAfterRender(firstRender);
            if (firstRender || dataChanged)
            {
                var dotNetReference = DotNetObjectReference.Create(this);
                JS.InvokeVoidAsync("getDataGridViewRecordPageCount", dotNetReference, "dataView");
                JS.InvokeVoidAsync("getInitialDataGridViewRecordPageCount", dotNetReference, "dataView");
                dataChanged = false;
            }
        }

        protected override void OnInitialized()
        {
            base.OnInitialized();
            RrStateService.OnComponentChange += UpdateView;
        }

        private void UpdateView()
        {
            InitializeData(Data);
        }

        [JSInvokable("SetElementTopPosition")]
        public void GetPageSizeFromWindowHeightAndTableTopPosition(int count)
        {
            pageSize = count;
            StateHasChanged();
        }

        public string GetColumnPropertyExpression(string name, Type type)
        {
            var expression = $@"it[""{name}""].ToString()";

            if (type == typeof(int))
            {
                return $"int.Parse({expression})";
            }
            else if (type == typeof(DateTime))
            {
                return $"DateTime.Parse({expression})";
            }
            else if (type.IsEnum)
            {
                return $@"Int32(Enum.Parse(it[""{name}""].GetType(), {expression}))";
            }

            return expression;
        }

        private async Task OnColumnResizedHandler(DataGridColumnResizedEventArgs<IDictionary<string, object>> args)
        {
            if (!string.IsNullOrEmpty(tableType))
            {
                var newColumnWidth = new Models.ColumnKVP<string, double>(args.Column.Title, args.Width);
                if (!columnUserPreferences.TableColumnWidth.ContainsKey(tableType))
                {
                    columnUserPreferences.TableColumnWidth[tableType] = new List<Models.ColumnKVP<string, double>>();
                }

                bool updated = false;
                foreach (var columnWidth in columnUserPreferences.TableColumnWidth[tableType])
                {
                    if (columnWidth.Key == args.Column.Title)
                    {
                        columnWidth.Value = args.Width;
                        updated = true;
                        break;
                    }
                }

                if (!updated)
                {
                    columnUserPreferences.TableColumnWidth[tableType].Add(newColumnWidth);
                }

                //PreferencesService.SaveColumnPreferences(columnUserPreferences);
            }
        }

        private async Task OnColumnReorderedHandler(DataGridColumnReorderedEventArgs<IDictionary<string, object>> args)
        {
            if (!string.IsNullOrEmpty(tableType))
            {
                var newColumnOrder = new Models.ColumnKVP<string, double>(args.Column.Title, args.NewIndex);
                if (!columnUserPreferences.TableColumnOrder.ContainsKey(tableType))
                {
                    columnUserPreferences.TableColumnOrder[tableType] = new List<Models.ColumnKVP<string, double>>();
                }

                bool updated = false;
                foreach (var columnOrder in columnUserPreferences.TableColumnOrder[tableType])
                {
                    if (columnOrder.Key == args.Column.Title)
                    {
                        columnOrder.Value = args.NewIndex;
                        updated = true;
                        break;
                    }
                }

                foreach (var columnOrder in columnUserPreferences.TableColumnOrder[tableType])
                {
                    if (columnOrder.Value < args.OldIndex && columnOrder.Value >= args.NewIndex && columnOrder.Key != args.Column.Title)
                    {
                        columnOrder.Value += 1;
                    }
                }

                if (!updated)
                {
                    columnUserPreferences.TableColumnOrder[tableType].Add(newColumnOrder);
                }

                //PreferencesService.SaveColumnPreferences(columnUserPreferences);
            }
        }

        private string GetColumnWidth(string columnName)
        {
            if (!string.IsNullOrEmpty(tableType) && columnUserPreferences.TableColumnWidth.ContainsKey(tableType))
            {
                foreach (var columnWidth in columnUserPreferences.TableColumnWidth[tableType])
                {
                    if (columnWidth.Key == columnName)
                    {
                        return $"{columnWidth.Value}px";
                    }
                }
            }
            return "200px"; // Default column width
        }

        private T? previousData;

        protected override void OnParametersSet()
        {
            base.OnParametersSet();
            if (Data != null && !EqualityComparer<T>.Default.Equals(Data, previousData))
            {
                InitializeData(Data);
                previousData = Data;
                dataChanged = true;
            }
        }

        private void InitializeData(T dataToDisplay)
        {
            //columnUserPreferences = PreferencesService.LoadColumnPreferences();
            if (dataToDisplay is List<object> listData)
            {
                Initialize("downloadsTable", listData);
            }
            if (!string.IsNullOrEmpty(tableType) && !columnUserPreferences.TableColumnSizes.ContainsKey(tableType))
            {
                columnUserPreferences.TableColumnSizes[tableType] = new List<double>();
            }
            // Apply row filtering
            FilterRows();
        }

        private void Initialize(string tableType, object data)
        {
            this.tableType = tableType;
            InitializeColumns(data);
            InitializeDataContent(data);
        }

        private void InitializeColumns(object data)
        {
            columns.Clear();
            if (data is List<object> listData && listData.Any())
            {
                var firstItem = listData.First();
                if (firstItem is IDictionary<string, object> expandoDict)
                {
                    foreach (var key in expandoDict.Keys)
                    {
                        columns.Add(key, typeof(object));
                    }
                }
                else
                {
                    foreach (var prop in firstItem.GetType().GetProperties())
                    {
                        columns.Add(prop.Name, typeof(object));
                    }
                }
            }
            if (!string.IsNullOrEmpty(tableType) && columnUserPreferences.TableColumnOrder.ContainsKey(tableType))
            {
                var orderedColumns = new List<Models.ColumnKVP<string, Type>>();
                foreach (var columnOrder in columnUserPreferences.TableColumnOrder[tableType])
                {
                    if (columns.ContainsKey(columnOrder.Key))
                    {
                        orderedColumns.Add(new Models.ColumnKVP<string, Type>(columnOrder.Key, columns[columnOrder.Key]));
                    }
                }

                int nextIndex = columnUserPreferences.TableColumnOrder[tableType].Count;
                foreach (var column in columns)
                {
                    if (!orderedColumns.Any(c => c.Key == column.Key))
                    {
                        orderedColumns.Add(new Models.ColumnKVP<string, Type>(column.Key, column.Value));
                    }
                }

                orderedColumns = orderedColumns.OrderBy(c =>
                {
                    var orderPair = columnUserPreferences.TableColumnOrder[tableType].FirstOrDefault(p => p.Key == c.Key);
                    return orderPair != null ? orderPair.Value : int.MaxValue;
                }).ToList();

                columns = orderedColumns.ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
            }
        }

        private void InitializeDataContent(object data)
        {
            this.data = new List<IDictionary<string, object>>();
            if (data is List<object> listData)
            {
                foreach (var item in listData)
                {
                    if (item is IDictionary<string, object> expandoDict)
                    {
                        this.data.Add(new Dictionary<string, object>(expandoDict));
                    }
                    else
                    {
                        var itemDict = new Dictionary<string, object>();
                        foreach (var prop in item.GetType().GetProperties())
                        {
                            itemDict[prop.Name] = prop.GetValue(item);
                        }
                        this.data.Add(itemDict);
                    }
                }
            }
            // Apply row filtering
            FilterRows();
        }

        private void FilterRows()
        {
            if (data is not null)
            {
                filteredData = data.Where(ShouldIncludeRow).ToList();
            }
        }

        private bool ShouldIncludeColumn(string columnName)
        {
            if (Settings != null)
            {
                foreach (var setting in Settings)
                {
                    foreach (var condition in setting.Conditions)
                    {
                        if (condition.EffectType == Models.SettingEffectType.Column)
                        {
                            if (condition.Type == Models.ColumnConditionType.ColumnExists && condition.ColumnName == columnName)
                            {
                                return setting.Active ? !condition.Value : condition.Value;
                            }
                        }
                    }

                }
            }
            return true;
        }

        private bool ShouldIncludeRow(IDictionary<string, object> row)
        {
            if (Settings != null)
            {
                foreach (var setting in Settings)
                {
                    foreach (var condition in setting.Conditions)
                    {
                        if (condition.EffectType == Models.SettingEffectType.Row)
                        {
                            if (condition.Type == Models.ColumnConditionType.ValueEquals && row.TryGetValue(condition.ColumnName, out var value))
                            {
                                // Ensure the value is correctly compared as a string
                                if (value != null && value.ToString() == condition.ColumnValue)
                                {
                                    return setting.Active ? !condition.Value : condition.Value;
                                }
                            }
                        }
                    }
                }
            }
            return true;
        }
    }
}
