using IPeople.Roadrunner.Razor.Models;
using Microsoft.AspNetCore.Components;

namespace IPeople.Roadrunner.Razor.Components
{
    public partial class RrButton
    {
        [Parameter]
        public string Id { get; set; } = "";

        [Parameter]
        public string Style { get; set; } = "";

        [Parameter]
        public EventCallback OnClick { get; set; }

        [Parameter]
        public ButtonSizes Size { get; set; } = ButtonSizes.Medium;

        [Parameter]
        public RenderFragment ChildContent { get; set; }
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
