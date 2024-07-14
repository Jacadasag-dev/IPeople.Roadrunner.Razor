using IPeople.Roadrunner.Razor.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace IPeople.Roadrunner.Razor.Components
{
    public partial class RrPopup : IRrComponentBase
    {
        [Parameter]
        public string? Id { get; set; }

        [Parameter]
        public string? Tag { get; set; }

        [Parameter]
        public bool Visible { get; set; } = true;

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

                await JS.InvokeVoidAsync("makeDraggable", Popup.Id);
            }
        }

        private void HandlePopupClose()
        {
            if (Popup is null)
                return;

            RrStateService.RemoveComponent<RrPopup>(Popup.Id);
            RrStateService.RefreshComponents();
        }
    }
}
