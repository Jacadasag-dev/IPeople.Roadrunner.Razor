using IPeople.Roadrunner.Razor.Models;
using IPeople.Roadrunner.Razor.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        /// <summary>
        /// Used for interfacing with javascript for sizing the body
        /// </summary>
        public class Bounds
        {
            public double Top { get; set; }
            public double Left { get; set; }
            public double Width { get; set; }
            public double Height { get; set; }
            public double Right { get; set; }
            public double Bottom { get; set; }
        }

        private Bounds? bounds;

        private class RrPanelDto
        {
            public string? Id { get; set; }
            public string? Size { get; set; }
            public string? PType { get; set; }
            public bool Latching { get; set; }
            public string? LatchingType { get; set; }
            public int MinLatchingWidth { get; set; }
            public DotNetObjectReference<RrPanel>? DotNetObjectReference { get; set; }
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {

                //String Array of panelIds
                //string?[]? panelIds = RrStateService.GetComponentsByTag<RrPanel>("Panel")?.Select(panel => panel.Id).ToArray();
                //string?[]? panelInitialSizes = RrStateService.GetComponentsByTag<RrPanel>("Panel")?.Select(panel => panel.Size).ToArray();

                //if (panelIds is null || panelInitialSizes is null)
                //    return;

                //var combinedIdsAndSizes = panelIds?.Zip(panelInitialSizes, (id, size) => new { Id = id, Size = size }).ToArray();



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
                    MinLatchingWidth = panel.LatchingPanelsMininmumAdjustmentSize,
                    DotNetObjectReference = panel.dotNetReference
                }).ToList();



                await JS.InvokeVoidAsync("registerPageAndPanels", $"{Id}-body", panelDtos);

            }
        }

        [JSInvokable]
        public void UpdateBounds(Bounds newBounds)
        {
            bounds = newBounds;
            RrStateService.AppGlobalVariables.BodyBounds = new();
            RrStateService.AppGlobalVariables.BodyBounds.TopPosition = (int)bounds.Top;
            RrStateService.AppGlobalVariables.BodyBounds.BottomPosition = (int)bounds.Bottom;
            RrStateService.AppGlobalVariables.BodyBounds.LeftPosition = (int)bounds.Left;
            RrStateService.AppGlobalVariables.BodyBounds.RightPosition = (int)bounds.Right;
            RrStateService.AppGlobalVariables.BodyBounds.Width = (int)bounds.Width;
            RrStateService.AppGlobalVariables.BodyBounds.Height = (int)bounds.Height;
            //StateHasChanged();
            //RrStateService.RefreshComponentsByTag("Panel");
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
