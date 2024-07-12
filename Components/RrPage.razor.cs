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
    public partial class RrPage
    {
        #region Parameters
        [Parameter]
        public RenderFragment Header { get; set; }

        [Parameter]
        public RenderFragment Body { get; set; }

        [Parameter]
        public RenderFragment Footer { get; set; }

        [Parameter]
        public string TopOffset { get; set; } = "0px";

        [Parameter]
        public string LeftOffset { get; set; } = "0px";

        [CascadingParameter]
        public string LatchingPanels { get; set; } = "400px";
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

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                var dotNetObjectReference = DotNetObjectReference.Create(this);
                await JS.InvokeVoidAsync("setupResizeListener", "pagebody", dotNetObjectReference);
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
            StateHasChanged();
            RrStateService.RefreshComponentsByTag("Panel");
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
