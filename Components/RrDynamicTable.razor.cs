using IPeople.Roadrunner.Razor.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace IPeople.Roadrunner.Razor.Components
{
    public partial class RrDynamicTable<TItem> : IRrComponentBase
    {
        #region Parameters
        [Parameter]
        public string? Id { get; set; }

        [Parameter]
        public string? Tag { get; set; }

        [Parameter]
        public bool Visible { get; set; } = true;

        [Parameter]
        public IEnumerable<TItem>? Items { get; set; }

        [Parameter]
        public List<string>? IncludedColumns { get; set; }

        [Parameter]
        public List<string>? ColumnWidths { get; set; }

        [Parameter]
        public RenderFragment<TItem>? Action { get; set; }

        [Parameter]
        public string ActionName { get; set; } = "Action";

        [Parameter]
        public string? ActionColumnWidth { get; set; }

        [Parameter]
        public string NullOrEmptyMessage { get; set; } = "No records found...";

        [Parameter]
        public bool Virtualize { get; set; } = false;
        #endregion

        private RrLoadingBase? loading;

        protected override void OnInitialized()
        {
            RrStateService.RefreshAllComponents += StateHasChanged;
            RrStateService.RefreshSpecificComponentsById += (ids) => { if (ids is not null && ids.Contains(Id ?? "")) { StateHasChanged(); } };
            RrStateService.RefreshSpecificComponentsByTag += (tags) => { if (tags is not null && tags.Contains(Tag ?? "")) { StateHasChanged(); } };
            RrStateService.LoadingStateChangeRequestById += (id, newLoading) => { if (id == Id) { loading = newLoading; StateHasChanged(); } };
            RrStateService.LoadingStateChangeRequestByTag += (tag, newLoading) => { if (tag == Tag) { loading = newLoading; StateHasChanged(); } };
            RrStateService.StopAllLoading += () => { if (loading is not null && loading.IsLoading) { loading = new(); StateHasChanged(); } };
        }

        private IEnumerable<(string Name, string Width)> GetColumnInfo(object item)
        {
            List<string>? propertyNames = GetPropertyNames(item)?.ToList();
            if (propertyNames is null)
                return Enumerable.Empty<(string Name, string Width)>();
            
            List<string> columnWidths = ColumnWidths ?? new List<string>();
            return propertyNames.Select((name, index) =>
            {
                string width = index < columnWidths.Count ? columnWidths[index] : "auto";
                return (name, width);
            });
        }

        private IEnumerable<string>? GetPropertyNames(object item)
        {
            // If IncludedColumns are provided, use them
            if (IncludedColumns != null && IncludedColumns.Any())
            {
                return IncludedColumns;
            }

            // If the item is a dictionary, return its keys
            if (item is IDictionary<string, object> expando)
            {
                return expando.Keys;
            }

            // If the item has properties of type Dictionary<string, object>, return their names
            var dictionaryPropertyNames = item.GetType().GetProperties()
                                              .Where(p => p.PropertyType == typeof(Dictionary<string, object>))
                                              .Select(p => p.Name);

            if (dictionaryPropertyNames.Any())
            {
                return dictionaryPropertyNames;
            }

            // If the item has private fields of type Dictionary<string, object>, return their keys
            var dictionaryField = item.GetType()
                                      .GetFields(System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                                      .FirstOrDefault(f => f.FieldType == typeof(Dictionary<string, object>));

            if (dictionaryField != null)
            {
                var dictionary = (Dictionary<string, object>?)dictionaryField.GetValue(item);
                return dictionary?.Keys;
            }

            // Default case: return public property names
            return item.GetType().GetProperties().Select(p => p.Name).ToList();
        }

        private IEnumerable<object?>? GetPropertyValues(object item)
        {
            // If the item is a dictionary, return its values formatted
            if (item is IDictionary<string, object> expando)
            {
                return IncludedColumns?.Select(col => expando.TryGetValue(col, out var value) ? FormatValue(col, value) : null)
                       ?? expando.Values.Select(value => FormatValue(string.Empty, value));
            }

            // Retrieve type of the item
            Type? itemType = item.GetType();
            // Get list of properties to include, defaulting to all property names
            List<string>? properties = IncludedColumns ?? itemType.GetProperties().Select(p => p.Name).ToList();

            // Handle properties of type Dictionary<string, object>
            List<PropertyInfo>? dictionaryProperties = itemType.GetProperties()
                                               .Where(p => p.PropertyType == typeof(Dictionary<string, object>))
                                               .ToList();

            if (dictionaryProperties.Any())
            {
                IEnumerable<object?>? dictionaryValues = dictionaryProperties.SelectMany(prop =>
                {
                    Dictionary<string, object>? dictionary = (Dictionary<string, object>?)prop.GetValue(item);
                    if (dictionary == null)
                    {
                        return Enumerable.Empty<object?>();
                    }

                    return IncludedColumns?.Select(col => dictionary.TryGetValue(col, out var value) ? FormatValue(col, value) : null)
                           ?? dictionary.Values.Select(value => FormatValue(string.Empty, value));
                });

                return dictionaryValues;
            }

            // Handle private fields of type Dictionary<string, object>
            FieldInfo? dictionaryField = itemType.GetFields(System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                                          .FirstOrDefault(f => f.FieldType == typeof(Dictionary<string, object>));

            if (dictionaryField != null)
            {
                Dictionary<string, object>? dictionary = (Dictionary<string, object>?)dictionaryField.GetValue(item);
                if (dictionary == null)
                {
                    return Enumerable.Empty<object?>();
                }

                return IncludedColumns?.Select(col => dictionary.TryGetValue(col, out var value) ? FormatValue(col, value) : null)
                       ?? dictionary.Values.Select(value => FormatValue(string.Empty, value));
            }

            // Default case: return values of public properties formatted
            return properties.Select(propName => FormatValue(propName, itemType.GetProperty(propName)?.GetValue(item)));
        }

        private object? FormatValue(string propertyName, object? value)
        {
            if (propertyName.StartsWith("is", StringComparison.OrdinalIgnoreCase))
            {
                if (value is int intValue)
                {
                    return intValue == 1 ? true : intValue == 0 ? false : (object)intValue;
                }
            }
            return value ?? null;
        }

        private void HandleColumnHeaderClicked((string Name, string Width) columnInfo)
        {
            Console.WriteLine($"Column header clicked: {columnInfo.Name}");
        }

    }
}
