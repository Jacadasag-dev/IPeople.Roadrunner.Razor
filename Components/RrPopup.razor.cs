using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace IPeople.Roadrunner.Razor.Components
{
    public partial class RrPopup
    {
        [Parameter]
        public string Style { get; set; } = "";
        [Parameter]
        public Models.RrPopup? Popup { get; set; }

        [Parameter]
        public RenderFragment? ChildContent { get; set; }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                if (Popup is null)
                    return;

                await JS.InvokeVoidAsync("makeDraggable", Popup.Identifier);
            }
        }

        private void HandlePopupClose()
        {
            if (Popup is null)
                return;

            RrStateService.RemoveComponent(Popup);
            RrStateService.RefreshComponents();
        }
    }
}
