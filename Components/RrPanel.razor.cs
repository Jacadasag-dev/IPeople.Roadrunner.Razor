using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace IPeople.Roadrunner.Razor.Components
{
    public partial class RrPanel
    {
        [Parameter]
        public RenderFragment PanelTabs { get; set; }
        [Parameter]
        public RenderFragment PanelContent { get; set; }
        [Parameter]
        public RenderFragment PanelSettings { get; set; }
        [Parameter]
        public string Header { get; set; }
        [Parameter]
        public bool EnableScroll { get; set; } = true;
        protected override void OnInitialized()
        {
            RrStateService.OnComponentChange += StateHasChanged;
        }
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                await JS.InvokeVoidAsync("observeElementWidth", $"rr-panel-header-{Header}", "rr-panel-header-narrow", "rr-panel-header-long", PanelTabs is not null ? 500 : 950, 950);
            }
        }
    }
}
