using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace IPeople.Roadrunner.Razor.Components
{
    public partial class RrDropdown<T> : ComponentBase
    {
        [Parameter]
        public string? Id { get; set; }

        [Parameter]
        public bool Disabled { get; set; } = false;

        [Parameter]
        public bool Visible { get; set; } = true;

        [Parameter]
        public string MinWidth { get; set; } = "50px"; // Default value

        [Parameter]
        public string MaxHeight { get; set; } = "230px"; // Default value

        [Parameter]
        public string Placeholder { get; set; } = "Select";

        [Parameter]
        public IEnumerable<object>? Items { get; set; }

        [Parameter]
        public string Style { get; set; } = "";

        [Parameter]
        public bool Flashing { get; set; } = false;

        [Parameter]
        public List<string>? Effects { get; set; }

        [Parameter]
        public bool EffectsAll { get; set; } = false;

        [Parameter]
        public EventCallback<T> OnSelectionChanged { get; set; }

        [Parameter]
        public Models.RrDropdown? Dropdown { get; set; }

        private Models.RrDropdown? dropdownFromService;
        private IEnumerable<object> items = [];
        private bool visible;
        private List<T> processedItems = [];
        private T? selectedItem;
        private Models.UIStates dropdownUIState = Models.UIStates.Neutral;
        private string placeholder = "Select";
        private string? dropdownCssClass;
        private string? calculatedWidth;
        private string? exceptionMessage;

        /// <summary>
        /// Initializes the dropdown component whenever it is rendered.
        /// </summary>
        private void InitializeDropdown()
        {
            if (!string.IsNullOrEmpty(Id))
            {
                dropdownFromService = RrStateService.GetComponentById<Models.RrDropdown>(Id) as Models.RrDropdown;
            } 
            else if (Dropdown is not null)
            {
                dropdownFromService = RrStateService.GetComponent<Models.RrDropdown>(Dropdown) as Models.RrDropdown;
                if (dropdownFromService is not null)
                {
                    Id = dropdownFromService.Identifier;
                }
            }
            if (dropdownFromService is null)
            {
                if (!string.IsNullOrEmpty(Id))
                {
                    RrStateService.RegisterComponentById<Models.RrDropdown>(Id);
                    RrStateService.RefreshAllComponents += StateHasChanged;
                    dropdownFromService = RrStateService.GetComponentById<Models.RrDropdown>(Id) as Models.RrDropdown;
                } else
                {
                    exceptionMessage = "ERROR:id_is_required";
                }
            } 
            bool notNull = dropdownFromService is not null;

            if (notNull)
            {
                if (dropdownFromService?.Items is not null && dropdownFromService.Items.Any())
                {
                    items = dropdownFromService.Items;
                }
                else if (Items is not null && Items.Any())
                {
                    items = Items;
                }
                else
                {
                    items = new List<object>();
                }
            }
            visible = notNull ? dropdownFromService?.Visible ?? Visible : Visible;
            selectedItem = notNull ? (dropdownFromService?.SelectedItem is T item ? item : default) : default;
            placeholder = notNull ? dropdownFromService?.PlaceHolder ?? Placeholder : Placeholder;
            processedItems = GetProcessedItems(items)?.ToList() ?? new List<T>();
            if (processedItems is null || !processedItems.Any())
            {
                processedItems = items?.Cast<T>().ToList() ?? new List<T>();
            }




            dropdownCssClass = GetDropdownCssClassAndWidth(dropdownUIState);

            if (notNull) RrStateService.SynchronizeComponent<Models.RrDropdown>(dropdownFromService);
        }

        /// <summary>
        /// Registers the dropdown with the JS function to handle the click outside of the dropdown.
        /// </summary>
        /// <param name="firstRender"></param>
        /// <returns></returns>
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                // Register the dropdown with the JS function to handle the click outside of the dropdown
                await JS.InvokeVoidAsync("registerDropdown", Id, DotNetObjectReference.Create(this));
            }
        }

        /// <summary>
        /// Handles the selected item and invokes the OnSelectionChanged event passing the item.
        /// </summary>
        /// <param name="item"></param>
        private async void HandleItemSelected(T? item)
        {
            selectedItem = item;
            string selectedItemString = RrStateService.GetDisplayValue(selectedItem) ?? "Null";
            if (dropdownFromService is not null && selectedItem is not null)
            {
                RrStateService.SetComponentProperty<Models.RrDropdown, object>(dropdownFromService, c => c.SelectedItem, selectedItem);
            }
            
            SetSelectedWidth();
            StateHasChanged();
            if (Effects is not null && Effects.Any())
            {
                RrStateService.RefreshComponentsById(Effects);
            }
            else if (EffectsAll)
            {
                RrStateService.RefreshComponents();
            }
            
            await OnSelectionChanged.InvokeAsync(selectedItem);
        }

        /// <summary>
        /// Called when the dropdown is clicked to handle the dropdown state and invoke the JS function to handle the dropdown click.
        /// </summary>
        private async void HandleDropdownClicked()
        {
            // Invoke the JS function to handle the dropdown click so that it doesn't get closed when clicking on the dropdown itself
            await JS.InvokeVoidAsync("invokeHandleDropdownClicked", Id);
            ToggleDropdownState();
        }

        /// <summary>
        /// Makes sure to toggle the dropdown state properly for animations when the dropdown is in specific states.
        /// </summary>
        private void ToggleDropdownState()
        {
            if (dropdownUIState == Models.UIStates.Neutral)
            {
                dropdownUIState = Models.UIStates.Expanded;
            }
            else if (dropdownUIState == Models.UIStates.Expanded)
            {
                dropdownUIState = Models.UIStates.Collapsed;
            }
            else if (dropdownUIState == Models.UIStates.Collapsed)
            {
                dropdownUIState = Models.UIStates.Expanded;
            }
            StateHasChanged();
        }

        /// <summary>
        /// Sets the width of the dropdown based on the maximum width of the items in the dropdown.
        /// </summary>
        private void SetMaxWidth()
        {
            if (RrStateService != null)
            {
                if (items is not null)
                {
                    bool somethingIsSelected = selectedItem is not null;

                    double placeholderWidth = CalculateWidth(placeholder.Length);

                    double? exceptionMessageWidth = null;
                    if (!string.IsNullOrEmpty(exceptionMessage))
                        exceptionMessageWidth = CalculateWidth(exceptionMessage.Length);

                    var myItems = items
                        .Where(item => item != null)
                        .Select(item => new
                        {
                            Item = RrStateService.GetDisplayValue(item),
                            Width = CalculateWidth((RrStateService.GetDisplayValue(item) ?? placeholder).Length)
                        })
                        .ToList();

                    string maxItem = myItems
                        .OrderByDescending(i => i.Width)
                        .FirstOrDefault()?.Item ?? placeholder;

                    string theMaxItem = maxItem;
                    if (!somethingIsSelected)
                    {
                        if (exceptionMessageWidth is not null)
                        {
                            theMaxItem = exceptionMessage ?? placeholder;
                        }
                        else
                        {
                            theMaxItem = maxItem.Length > placeholder.Length ? maxItem : placeholder;
                        }
                    }

                    calculatedWidth = $"{Math.Round(ModifyWidthBasedOnWord(theMaxItem), 1)}px";
                }
            }
        }

        /// <summary>
        /// Sets the width of the dropdown based on the selected item.
        /// Accounts for words with no 'i's being longer than words with 'i's like SUMMARY
        /// </summary>
        private void SetSelectedWidth()
        {
            if (RrStateService != null)
            {
                string? word = RrStateService.GetDisplayValue(selectedItem);
                if (!string.IsNullOrEmpty(word))
                {
                    calculatedWidth = $"{Math.Round(ModifyWidthBasedOnWord(word), 1)}px";
                }
            }
        }

        /// <summary>
        /// Modifies the width of the dropdown based on the word selected.
        /// </summary>
        /// <param name="word"></param>
        /// <returns></returns>
        private double ModifyWidthBasedOnWord(string word)
        {
            double width = CalculateWidth(word.Length);
            if (word.Length < 9 && !word.Contains("i"))
            {
                width += 10;
            }
            if (word.Contains("."))
            {
                width -= (word.Count(c => c == '.') * 10);
            }
            return width;
        }

        /// <summary>
        /// Calculates the width of the dropdown based on the number of characters in the string and a scaling factor.
        /// </summary>
        /// <param name="initialNumber"></param>
        /// <returns></returns>
        static double CalculateWidth(double initialNumber)
        {
            // The formula for the width of the dropdown based on the number of characters in the string (thanks algebra teacher in 9th grade)
            var constantC = 5.5;
            var scalingFactorK = 1.13;
            return initialNumber + constantC * Math.Pow(initialNumber + constantC, scalingFactorK);
        }

        /// <summary>
        /// Collapses the dropdown if it is open and you click outside of the dropdown.
        /// </summary>
        [JSInvokable]
        public void CollapseDropdownIfOpen()
        {
            if (dropdownUIState == Models.UIStates.Expanded)
            {
                dropdownUIState = Models.UIStates.Collapsed;
            }
            StateHasChanged();
        }

        /// <summary>
        /// Checks to see if something is selected, then removes it from the dropdown list so that it doesn't show up twice.
        /// </summary>
        private List<T>? GetProcessedItems(IEnumerable<object> myList)
        {
            if (selectedItem is not null && myList is not null && myList.Any())
            {
                return myList
                    .Where(item => item is T typedItem && typedItem is not null && !typedItem.Equals(selectedItem))
                    .Cast<T>()
                    .ToList();
            }
            return null;
        }

        /// <summary>
        /// Sets the dropdown to the correct CSS class based on the current state.
        /// </summary>
        /// <param name="currentState"></param>
        /// <returns></returns>
        private string GetDropdownCssClassAndWidth(Models.UIStates currentState)
        {
            if (currentState == Models.UIStates.Expanded)
            {
                SetMaxWidth();
                return "expanded";
            }
            else if (currentState == Models.UIStates.Collapsed)
            {
                SetSelectedWidth();
                return "minimized";
            }
            else if (currentState == Models.UIStates.Neutral)
            {
                SetSelectedWidth();
                return "";
            }
            return "invalid-state";
        }
    }
}
