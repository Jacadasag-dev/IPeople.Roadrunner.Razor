using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace IPeople.Roadrunner.Razor.Components
{
    public partial class RrPopup
    {
        [Parameter]
        public string Style { get; set; } = "";
        [Parameter]
        public Models.RrPopup Popup { get; set; }

        [Parameter]
        public RenderFragment ChildContent { get; set; }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                await JS.InvokeVoidAsync("makeDraggable", Popup.Identifier);
            }
        }

        private void HandlePopupClose()
        {
            RrStateService.RemoveComponent(Popup);
            RrStateService.RefreshComponents();
        }
    }
}
