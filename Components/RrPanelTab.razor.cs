﻿using Microsoft.AspNetCore.Components;

namespace IPeople.Roadrunner.Razor.Components
{
    public partial class RrPanelTab
    {
        [Parameter]
        public string Style { get; set; } = "";
        [Parameter]
        public Models.RrPanelTab Tab { get; set; }

        private void HandleTabSelected()
        {
            RrStateService.SetSelectedTab(Tab);
            RrStateService.SetTabSettingsExpandState(Tab, Models.UIStates.Neutral);
            RrStateService.RefreshComponents();
        }
    }
}
