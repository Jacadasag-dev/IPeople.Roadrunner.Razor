using Microsoft.AspNetCore.Components;

namespace IPeople.Roadrunner.Razor.Components
{
    public partial class RrPanelTab
    {
        [Parameter]
        public Models.RrPanelTab Tab { get; set; }

        private void HandleTabSelected()
        {
            RrStateService.SetSelectedTab(Tab);
            RrStateService.SetTabSettingsExpandState(Tab, Models.SettingUIStates.Neutral);
            RrStateService.RefreshComponents();
        }
    }
}
