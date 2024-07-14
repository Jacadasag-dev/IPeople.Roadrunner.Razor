using IPeople.Roadrunner.Razor.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace IPeople.Roadrunner.Razor.Components
{
    public partial class RrDropdown<T> : IRrComponentBase
    {
        #region Parameters
        [Parameter]
        public string? Id { get; set; }

        [Parameter]
        public string? Tag { get; set; }

        [Parameter]
        public bool Disabled { get; set; } = false;

        [Parameter]
        public bool Visible { get; set; } = true;

        [Parameter]
        public string MinWidth { get; set; } = "50px"; // Default value

        [Parameter]
        public string MaxHeight { get; set; } = "230px"; // Default value

        [Parameter]
        public string? Placeholder { get; set; } = "Select";

        [Parameter]
        public IEnumerable<object>? Items { get; set; }

        [Parameter]
        public string Style { get; set; } = "";

        [Parameter]
        public bool Flashing { get; set; } = false;

        [Parameter]
        public List<string>? EffectsById { get; set; }

        [Parameter]
        public List<string>? EffectsByTag { get; set; }

        [Parameter]
        public bool EffectsAll { get; set; } = false;

        [Parameter]
        public string? Label { get; set; }

        [Parameter]
        public string? LabelStyling { get; set; }

        [Parameter]
        public EventCallback<T> OnSelectionChanged { get; set; }

        [Parameter]
        public T? SelectedItem { get; set; }
        #endregion

        #region Private Fields
        private List<T>? processedItems = [];
        private UIStates dropdownUIState = UIStates.Neutral;
        private string? dropdownCssClass;
        private string? calculatedWidth;
        #endregion

        /// <summary>
        /// Initializes the dropdown component whenever it is rendered.
        /// </summary>
        private void InitializeDropdown()
        {
            if (string.IsNullOrEmpty(Id)) throw new Exception($"The \"Id\" parameter must be defined and set as it is required for the {this.GetType().ToString()} component to function.");
            Id = RrStateService.GetPropertyIfIsNotNullElseIfNullSetToNewValueAndReturnNewValue<RrDropdown<T>, string>(this, p => p.Id, Id);
            Tag = RrStateService.GetPropertyIfIsNotNullElseIfNullSetToNewValueAndReturnNewValue<RrDropdown<T>, string>(this, p => p.Tag, Tag);
            Visible = RrStateService.GetPropertyIfIsNotNullElseIfNullSetToNewValueAndReturnNewValue<RrDropdown<T>, bool>(this, p => p.Visible, Visible);
            Placeholder = RrStateService.GetPropertyIfIsNotNullElseIfNullSetToNewValueAndReturnNewValue<RrDropdown<T>, string>(this, p => p.Placeholder, Placeholder);
            Items = RrStateService.GetPropertyIfIsNotNullElseIfNullSetToNewValueAndReturnNewValue<RrDropdown<T>, IEnumerable<object>>(this, p => p.Items, Items);
            Label = RrStateService.GetPropertyIfIsNotNullElseIfNullSetToNewValueAndReturnNewValue<RrDropdown<T>, string>(this, p => p.Label, Label);
            SelectedItem = RrStateService.GetPropertyIfIsNotNullElseIfNullSetToNewValueAndReturnNewValue<RrDropdown<T>, T>(this, p => p.SelectedItem, SelectedItem);
            processedItems = GetProcessedItems(Items)?.ToList();
            if (processedItems is null || !processedItems.Any())
                processedItems = Items?.Cast<T>().ToList();

            dropdownCssClass = GetDropdownCssClassAndWidth(dropdownUIState);
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
            SelectedItem = item;
            string selectedItemString = RrStateService.GetDisplayValue(SelectedItem) ?? "Null";
            RrStateService.SetComponentPropertyById<RrDropdown<T>, object>(Id, c => c.SelectedItem, SelectedItem);
            
            SetSelectedWidth();
            if (EffectsAll)
            {
                RrStateService.RefreshComponents();
            }
            else if (EffectsById is not null && EffectsById.Any())
            {
                RrStateService.RefreshComponentsById(EffectsById);
            }
            else if (EffectsByTag is not null && EffectsByTag.Any())
            {
                RrStateService.RefreshComponentsByTag(EffectsByTag);
            }
            await OnSelectionChanged.InvokeAsync(SelectedItem);
            StateHasChanged();
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
            if (RrStateService is null || Items is null)
                return;
                
            bool somethingIsSelected = SelectedItem is not null;
            double placeholderWidth = CalculateWidth(Placeholder!.Length);
            var myItems = Items
                .Where(item => item != null)
                .Select(item => new
                {
                    Item = RrStateService.GetDisplayValue(item),
                    Width = CalculateWidth((RrStateService.GetDisplayValue(item) ?? Placeholder).Length)
                })
                .ToList();

            string maxItem = myItems
                .OrderByDescending(i => i.Width)
                .FirstOrDefault()?.Item ?? Placeholder ?? "Select";

            string theMaxItem = maxItem;
            if (!somethingIsSelected)
                theMaxItem = maxItem.Length > Placeholder!.Length ? maxItem : Placeholder;

            calculatedWidth = $"{Math.Round(CalculateWordWidth(theMaxItem), 1)}px";
        }

        /// <summary>
        /// Sets the width of the dropdown based on the selected item.
        /// Accounts for words with no 'i's being longer than words with 'i's like SUMMARY
        /// </summary>
        private void SetSelectedWidth()
        {
            if (RrStateService is null)
                return;

            string? word = RrStateService.GetDisplayValue(SelectedItem);
            if (!string.IsNullOrEmpty(word))
                calculatedWidth = $"{Math.Round(CalculateWordWidth(word), 1)}px";
        }

        /// <summary>
        /// Modifies the width of the dropdown based on the word selected.
        /// </summary>
        /// <param name="word"></param>
        /// <returns></returns>
        public double CalculateWordWidth(string word)
        {
            double width = CalculateWidth(word.Length);

            width = word switch
            {
                string w when w.Length < 9 && !w.Contains("i") => width + 10,
                string w when w.Contains(".") => width - (w.Count(c => c == '.') * 10),
                string w when w.IndexOf("w", StringComparison.OrdinalIgnoreCase) >= 0 => width + (w.Count(c => char.ToLower(c) == 'w') * 5),
                _ => width
            };

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
            double constantC = 5.5;
            double scalingFactorK = 1.13;
            return initialNumber + constantC * Math.Pow(initialNumber + constantC, scalingFactorK);
        }

        /// <summary>
        /// Collapses the dropdown if it is open and you click outside of the dropdown.
        /// </summary>
        [JSInvokable]
        public void CollapseDropdownIfOpen()
        {
            if (dropdownUIState == UIStates.Expanded)
                dropdownUIState = UIStates.Collapsed;

            StateHasChanged();
        }

        /// <summary>
        /// Checks to see if something is selected, then removes it from the dropdown list so that it doesn't show up twice.
        /// </summary>
        private List<T>? GetProcessedItems(IEnumerable<object>? myList)
        {
            if (SelectedItem is not null && myList is not null && myList.Any())
            {
                return myList
                    .Where(item => item is T typedItem && typedItem is not null && !typedItem.Equals(SelectedItem))
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
        private string GetDropdownCssClassAndWidth(UIStates currentState)
        {
            switch (currentState)
            {
                case UIStates.Expanded:
                    SetMaxWidth();
                    return "expanded";

                case UIStates.Collapsed:
                    SetSelectedWidth();
                    return "minimized";

                case UIStates.Neutral:
                    SetSelectedWidth();
                    return "";

                default:
                    return "invalid-state";
            }
        }
    }
}
