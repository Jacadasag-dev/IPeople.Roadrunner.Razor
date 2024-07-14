using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components;
using IPeople.Roadrunner.Razor.Models;

namespace IPeople.Roadrunner.Razor.Components
{
    public partial class RrInput : IRrComponentBase
    {
        #region Parameters
        [Parameter]
        public string? Id { get; set; }

        [Parameter]
        public string? Tag { get; set; }

        [Parameter]
        public bool Visible { get; set; } = true;

        [Parameter]
        public string? Placeholder { get; set; } = "Start typing...";

        [Parameter]
        public string MaxWidth { get; set; } = "620px";

        [Parameter]
        public string MinWidth { get; set; } = "0px";

        [Parameter]
        public string Style { get; set; } = "";

        [Parameter]
        public bool DeBounce { get; set; } = true;

        [Parameter]
        public string? Text { get; set; } = "";
        
        [Parameter]
        public EventCallback<string> OnChangedText { get; set; }

        [Parameter]
        public EventCallback<(KeyboardEventArgs, string, Models.RrInput)> OnKeyDown { get; set; }

        [Parameter]
        public EventCallback OnInputClick { get; set; }
        #endregion

        #region Private Fields
        private Models.RrInput? inputFromService;
        private string? exceptionMessage;
        private bool visible;
        private bool deBounce;
        private string? placeholder;
        private string? inputText;
        private string? instantInputText;
        private CancellationTokenSource debounceCts = new CancellationTokenSource();
        #endregion

        private void InitializeInput()
        {
            Id = RrStateService.GetPropertyIfIsNotNullElseIfNullSetToNewValueAndReturnNewValue<RrInput, string>(this, p => p.Id, Id);
            Visible = RrStateService.GetPropertyIfIsNotNullElseIfNullSetToNewValueAndReturnNewValue<RrInput, bool>(this, p => p.Visible, Visible);
            Tag = RrStateService.GetPropertyIfIsNotNullElseIfNullSetToNewValueAndReturnNewValue<RrInput, string>(this, p => p.Tag, Tag);
            Placeholder = RrStateService.GetPropertyIfIsNotNullElseIfNullSetToNewValueAndReturnNewValue<RrInput, string>(this, p => p.Placeholder, Placeholder);
            DeBounce = RrStateService.GetPropertyIfIsNotNullElseIfNullSetToNewValueAndReturnNewValue<RrInput, bool>(this, p => p.DeBounce, DeBounce);
            inputText = RrStateService.GetPropertyIfIsNotNullElseIfNullSetToNewValueAndReturnNewValue<RrInput, string>(this, p => p.Text, Text);
            instantInputText = inputText;
        }

        private async void HandleOnKeyDown(KeyboardEventArgs e)
        {
            if (string.IsNullOrEmpty(inputText)) return;
            await OnKeyDown.InvokeAsync((e, inputText, inputFromService));
        }

        private async void OnTextChanged(ChangeEventArgs e)
        {
            string? newText = e.Value?.ToString();
            if (string.IsNullOrEmpty(newText)) return;
            if (deBounce)
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
                inputText = newText;
                RrStateService.SetComponentPropertyById<Models.RrInput, string>(Id, c => c.Text, inputText);
                await OnChangedText.InvokeAsync(inputText);
            }
            else
            {
                inputText = newText;
                RrStateService.SetComponentPropertyById<Models.RrInput, string>(Id, c => c.Text, inputText);
                await OnChangedText.InvokeAsync(inputText);
            }
        }
    }
}
