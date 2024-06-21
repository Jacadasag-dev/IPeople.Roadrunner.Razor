using IPeople.Roadrunner.Razor.Models;
using Microsoft.AspNetCore.Components;

namespace IPeople.Roadrunner.Razor.Components
{
    public partial class RrPanelSettings
    {
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
            var updatedTab = (RrStateService.GetComponent<Models.RrPanel>(Tab.Panel) as Models.RrPanel).Tabs.FirstOrDefault(t => t.Name == Tab.Name);
            if (updatedTab is not null)
            {
                var panelPreference = new PanelSettingsPreferences();
                panelPreference.TabSettings.Add(updatedTab.Name, updatedTab.Settings);
                //PreferencesService.SavePanelPreferences(panelPreference);
            }
            RrStateService.RefreshComponents();
        }
    }
}
