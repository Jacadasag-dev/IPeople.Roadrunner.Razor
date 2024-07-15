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
        public string? LatchingPanelInitialSize { get; set; } = "400px";

        [Parameter]
        public int LatchingPanelsMininmumAdjustmentSize { get; set; } = 200;

        [Parameter]
        public LatchingTypes LatchingType { get; set; }
        #endregion

        private class RrPanelDto
        {
            public string? Id { get; set; }
            public string? Size { get; set; }
            public string? PType { get; set; }
            public bool Latching { get; set; }
            public string? LatchingType { get; set; }
            public int MinLatchingWidth { get; set; }
            public string? State { get; set; }
            public DotNetObjectReference<RrPanel>? DotNetObjectReference { get; set; }
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                List<RrPanel>? panels = RrStateService.GetComponentsByTag<RrPanel>("Panel");
                        if (panels is null)
                            return;

                var panelDtos = panels.Select(panel => new RrPanelDto
                {
                    Id = panel.Id,
                    Size = panel.Size,
                    PType = panel.PType.ToString(),
                    Latching = panel.Latching,
                    LatchingType = panel.LatchingType.ToString(),
                    MinLatchingWidth = LatchingPanelsMininmumAdjustmentSize,
                    State = panel.InitialState.ToString(),
                    DotNetObjectReference = panel.dotNetReference
                }).ToList();

                await JS.InvokeVoidAsync("registerPageAndPanels", $"{Id}-panels", panelDtos);
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
