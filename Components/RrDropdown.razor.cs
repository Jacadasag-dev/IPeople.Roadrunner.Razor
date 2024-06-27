using Microsoft.AspNetCore.Components;

namespace IPeople.Roadrunner.Razor.Components
{
    public partial class RrDropdown
    {
        [Parameter]
        public string Style { get; set; } = "";
        [Parameter] public EventCallback<string> OnNewSelection { get; set; }
        [Parameter] public Models.RrDropdown Dropdown { get; set; }
        protected override void OnInitialized()
        {
            RrStateService.RegisterComponent(Dropdown);
            RrStateService.OnComponentChange += StateHasChanged;
            RrStateService.OnClickOutOf += StateHasChanged;
        }
        private void HandleDropdownClicked()
        {
            RrStateService.ClickOutOfException(Dropdown);
            RrStateService.RefreshComponents();
        }
        private async void HandleItemSelected(string selectedItem)
        {
            RrStateService.SetComponentProperty<Models.RrDropdown, string>(Dropdown, c => c.Text, selectedItem);
            RrStateService.RefreshComponents();
            await OnNewSelection.InvokeAsync(selectedItem);
        }
    }
}
