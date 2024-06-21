using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components;

namespace IPeople.Roadrunner.Razor.Components
{
    public partial class RrInput
    {
        [Parameter] public EventCallback OnNewInput { get; set; }
        [Parameter] public EventCallback<KeyboardEventArgs> OnKeyDown { get; set; }
        [Parameter] public EventCallback OnInputClick { get; set; }
        [Parameter] public Models.RrInput Input { get; set; }
        private string inputText;
        protected override async Task OnInitializedAsync()
        {
            RrStateService.RegisterComponent(Input);
            RrStateService.OnComponentChange += HandleInputChangeRequest;
        }
        private void HandleInputChangeRequest()
        {
            Input.Text = RrStateService.GetComponent<Models.RrInput>(Input).Text;
            StateHasChanged(); // Re-render the component with the updated value
        }
        private CancellationTokenSource debounceCts = new CancellationTokenSource();
        private async void HandleOnKeyDown(KeyboardEventArgs e)
        {
            if (e.Key == "Enter")
            {
                if (Input.Identifier == "TableSearchInput" || Input.Identifier == "ColumnSearchInput")
                {
                    debounceCts.Cancel();
                    debounceCts = new CancellationTokenSource();
                    if (inputText == null) return;
                    RrStateService.SetComponentProperty<Models.RrInput, string>(Input, c => c.Text, inputText);
                    await OnNewInput.InvokeAsync();
                }
                else if (Input.Identifier == "SQLStatementInput")
                {
                    RrStateService.SetComponentProperty<Models.RrInput, string>(Input, c => c.Text, inputText);
                    await OnKeyDown.InvokeAsync();
                }
            }
            else if (e.Key == "ArrowDown")
            {
                //Future select search results row with down arrow
            }
        }
        private async void OnInputEntered(ChangeEventArgs e)
        {
            inputText = e.Value.ToString();
            if (Input.DoDeBounce)
            {
                debounceCts.Cancel();
                debounceCts = new CancellationTokenSource();
                try
                {
                    await Task.Delay(1000, debounceCts.Token);
                }
                catch (TaskCanceledException)
                {
                    return;
                }
                if (inputText == null) return;
                RrStateService.SetComponentProperty<Models.RrInput, string>(Input, c => c.Text, inputText);
                await OnNewInput.InvokeAsync();
            }
            else
            {
                if (inputText == null) return;
                RrStateService.SetComponentProperty<Models.RrInput, string>(Input, c => c.Text, inputText);
                await OnNewInput.InvokeAsync();
            }
        }
    }
}
