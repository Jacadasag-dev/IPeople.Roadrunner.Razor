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

        private Bounds bounds;

        public class Bounds
        {
            public double Top { get; set; }
            public double Left { get; set; }
            public double Width { get; set; }
            public double Height { get; set; }
            public double Right { get; set; }
            public double Bottom { get; set; }
        }

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



    }
}
