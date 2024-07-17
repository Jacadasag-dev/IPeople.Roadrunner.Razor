using IPeople.Roadrunner.Razor.Models;
using Microsoft.AspNetCore.Components;

namespace IPeople.Roadrunner.Razor.Components
{
    public partial class RrButton : IRrComponentBase
    {
        #region Parameters
        [Parameter]
        public string Id { get; set; } = "";

        [Parameter]
        public string? Tag { get; set; }

        [Parameter]
        public bool Visible { get; set; } = true;

        [Parameter]
        public string Style { get; set; } = "";

        [Parameter]
        public EventCallback OnClick { get; set; }

        [Parameter]
        public ComponentSizes Size { get; set; } = ComponentSizes.Medium;

        [Parameter]
        public RenderFragment? ChildContent { get; set; }
        #endregion

        private bool selected = false;
        private async void HandleOnClick()
        {
            selected = true;
            StateHasChanged();
            await OnClick.InvokeAsync();
        }
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (selected)
            {
                await Task.Delay(501);
                selected = false;
                StateHasChanged();
            }
        }
    }
}
