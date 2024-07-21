using IPeople.Roadrunner.Razor.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace IPeople.Roadrunner.Razor.Components
{
    public partial class RrPage : IRrComponentBase
    {
        #region Parameters
        [Parameter]
        public string? Id { get; set; }

        [Parameter]
        public RenderFragment Header { get; set; }

        [Parameter]
        public RenderFragment Panels { get; set; }

        [Parameter]
        public string? Tag { get; set; }

        [Parameter]
        public bool Visible { get; set; } = true;

        [Parameter]
        public RenderFragment Body { get; set; }

        [Parameter]
        public RenderFragment Footer { get; set; }

        [Parameter]
        public string TopOffset { get; set; } = "0px";

        [Parameter]
        public string LeftOffset { get; set; } = "0px";

        [Parameter] 
        public bool PanelLatching { get; set; }

        [Parameter]
        public string? LatchingPanelInitialSize { get; set; } = "400px";

        [Parameter]
        public bool LatchingDetachable { get; set; }

        [Parameter]
        public int LatchingPanelsMininmumAdjustmentSize { get; set; } = 200;

        [Parameter]
        public LatchingTypes LatchingType { get; set; }
        #endregion

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                if (Id is not null)
                    await RrStateService.RegisterContainingDivAndPanels(Id, "Panel", LatchingType, LatchingPanelsMininmumAdjustmentSize);

            }
        }

        private string GetBodyHeight()
        {
            if (Header is null && Footer is null)
            {
                return "height: 100%;";
            }
            else if (Header is not null && Footer is not null)
            {
                return "height: calc(100% - 118px);";
            }
            else if (Header is null)
            {
                return "height: calc(100% - 56px);";
            }
            else if (Footer is null)
            {
                return "height: calc(100% - 56px);";
            }
            return "invalid";
        }
    }
}
