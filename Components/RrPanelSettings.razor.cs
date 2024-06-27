using IPeople.Roadrunner.Razor.Models;
using Microsoft.AspNetCore.Components;

namespace IPeople.Roadrunner.Razor.Components
{
    public partial class RrPanelSettings
    {
        [Parameter]
        public string Style { get; set; } = "";
        [Parameter]
        public Models.RrPanelTab Tab { get; set; }

        [Parameter]
        public string Header { get; set; }

        [Parameter]
        public List<Models.RrPanelSetting> Settings { get; set; }

        private void ToggleSidebar()
        {
            RrStateService.ToggleTabSettingsExpand(Tab);
        }

        private void HandleOnSliderClick(string settingName)
        {
            RrStateService.ToggleTabSetting(Tab.Panel, Tab, settingName);
            RrStateService.UpdatePreference(Tab.Panel);
            RrStateService.RefreshComponents();
        }
    }
}
