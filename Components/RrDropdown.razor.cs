using IPeople.Roadrunner.Razor.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace IPeople.Roadrunner.Razor.Components
{
    public partial class RrDropdown
    {
        [Parameter]
        public string Id { get; set; } = Guid.NewGuid().ToString();

        [Parameter]
        public string MinWidth { get; set; } = "50px"; // Default value

        [Parameter]
        public string MaxHeight { get; set; } = "230px"; // Default value

        [Parameter]
        public string Placeholder { get; set; } = "Select";

        [Parameter]
        public IEnumerable<object> Items { get; set; }

        [Parameter]
        public string Style { get; set; } = "";

        [Parameter] public EventCallback<string> OnNewSelection { get; set; }

        [Parameter] public Models.RrDropdown Dropdown { get; set; }

        private string? calculatedWidth;
        string? dropdownCssClass;
        private Models.RrDropdown? dropdownInstance;
        private List<string> filteredItems = new List<string>();
        private SettingUIStates dropdownUIState = SettingUIStates.Neutral;

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                // Register the dropdown with the JS function to handle the click outside of the dropdown
                await JS.InvokeVoidAsync("registerDropdown", Id, DotNetObjectReference.Create(this));
                RrStateService.RegisterComponent(Dropdown);
                RrStateService.OnComponentChange += StateHasChanged;
            }
        }

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
            if (dropdownUIState == SettingUIStates.Neutral)
            {
                dropdownUIState = SettingUIStates.Expanded;
            }
            else if (dropdownUIState == SettingUIStates.Expanded)
            {
                dropdownUIState = SettingUIStates.Collapsed;
            }
            else if (dropdownUIState == SettingUIStates.Collapsed)
            {
                dropdownUIState = SettingUIStates.Expanded;
            }
            StateHasChanged();
        }

        private async void HandleItemSelected(string selectedItem)
        {
            RrStateService.SetComponentProperty<Models.RrDropdown, string>(Dropdown, c => c.Text, selectedItem);
            RrStateService.RefreshComponents();
            await OnNewSelection.InvokeAsync(selectedItem);
            SetSelectedWidth();
        }

        private void SetMaxWidth()
        {
            Models.RrDropdown? dropdown = RrStateService.GetComponent<Models.RrDropdown>(Dropdown ?? RrStateService.GetComponentById<Models.RrDropdown>(Id)) as Models.RrDropdown;
            if (dropdown is not null)
            {
                bool somethingIsSelected = !string.IsNullOrEmpty(dropdown.Text);

                string placeHolder = Placeholder ?? dropdown.PlaceHolder;
                double placeholderWidth = CalculateWidth(placeHolder.Length);

                var items = dropdown.Items?
                    .Where(item => item != null)
                    .Select(item => new { Item = item, Width = CalculateWidth(item.ToString().Length) })
                    .ToList();

                string maxItem = items?
                    .OrderByDescending(i => i.Width)
                    .FirstOrDefault()?.Item.ToString() ?? placeHolder;

                string theMaxItem = maxItem;
                if (!somethingIsSelected)
                {
                    theMaxItem = maxItem.Length > placeHolder.Length ? maxItem : placeHolder;
                }
                
                calculatedWidth = $"{Math.Round(ModifyWidthBasedOnWord(theMaxItem), 1)}px";
            }
        }

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
        /// Sets the width of the dropdown based on the selected item.
        /// Accounts for words with no 'i's being longer than words with 'i's like SUMMARY
        /// </summary>
        private void SetSelectedWidth()
        {
            var dropdown = RrStateService.GetComponent<Models.RrDropdown>(Dropdown ?? RrStateService.GetComponentById<Models.RrDropdown>(Id)) as Models.RrDropdown;
            string? word = string.IsNullOrEmpty(dropdown?.Text) ? dropdown?.PlaceHolder : dropdown?.Text ?? dropdown?.PlaceHolder;
            if (!string.IsNullOrEmpty(word))
            {
                calculatedWidth = $"{Math.Round(ModifyWidthBasedOnWord(word), 1)}px";
            }
        }

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
            var dropdown = RrStateService.GetComponent<Models.RrDropdown>(Dropdown ?? RrStateService.GetComponentById<Models.RrDropdown>(Id)) as Models.RrDropdown;
            if (dropdown is not null)
            {
                if (dropdownUIState == SettingUIStates.Expanded)
                {
                    dropdownUIState = SettingUIStates.Collapsed;
                }
                StateHasChanged();
            }
        }

        private void InitializeDropdownState()
        {
            var dropdown = RrStateService.GetComponent<Models.RrDropdown>(Dropdown ?? RrStateService.GetComponentById<Models.RrDropdown>(Id)) as Models.RrDropdown;
            if (dropdown is null)
            {
                RrStateService.RegisterComponent(new Models.RrDropdown(Id));
                dropdownInstance = RrStateService.GetComponentById<Models.RrDropdown>(Id) as Models.RrDropdown;
            }
            else
            {
                dropdownInstance = dropdown;
            }
            if (dropdownInstance is not null)
            {
                if (!string.IsNullOrEmpty(Placeholder))
                {
                    RrStateService.SetComponentProperty<Models.RrDropdown, string>(dropdownInstance, c => c.PlaceHolder, Placeholder);
                    dropdownInstance = RrStateService.GetComponent<Models.RrDropdown>(dropdownInstance) as Models.RrDropdown;
                }
                if (Items is not null && Items.Any())
                {
                    var convertedItems = Items.Select(item => item?.ToString())
                                              .Where(item => item is not null)
                                              .Cast<string>();

                    RrStateService.SetComponentProperty<Models.RrDropdown, IEnumerable<string>>(dropdownInstance, c => c.Items, convertedItems);
                    dropdownInstance = RrStateService.GetComponent<Models.RrDropdown>(dropdownInstance) as Models.RrDropdown;
                }
                filteredItems = dropdownInstance?.Items?
                    .Where(item => item is not null && item != dropdownInstance.Text)
                    .ToList() ?? new List<string>();

                if (dropdownUIState == Models.SettingUIStates.Expanded)
                {
                    dropdownCssClass = "expanded";
                    SetMaxWidth();
                }
                else if (dropdownUIState == Models.SettingUIStates.Collapsed)
                {
                    dropdownCssClass = "minimized";
                    SetSelectedWidth();
                }
                else if (dropdownUIState == Models.SettingUIStates.Neutral)
                {
                    dropdownCssClass = "";
                    SetSelectedWidth();
                }
            }
        }
    }
}
