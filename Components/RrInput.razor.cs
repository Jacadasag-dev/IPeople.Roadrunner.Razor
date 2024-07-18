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
        public bool MultiLine { get; set; } = false;

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
        public string MultiLineHeight { get; set; } = "90px";

        [Parameter]
        public string MultiLineWidth { get; set; } = "400px";

        [Parameter]
        public bool MultiLineLineNumbers { get; set; } = false;
        
        [Parameter]
        public EventCallback<string> OnChangedText { get; set; }

        [Parameter]
        public EventCallback<(KeyboardEventArgs, string, string)> OnKeyDown { get; set; }

        [Parameter]
        public EventCallback OnInputClick { get; set; }
        #endregion

        #region Private Fields
        private string? inputText;
        private string? instantInputText;
        private string lineNumbers = "1";
        private CancellationTokenSource debounceCts = new CancellationTokenSource();
        #endregion

        private void InitializeInput()
        {
            if (string.IsNullOrEmpty(Id)) throw new Exception($"The \"Id\" parameter must be defined and set as it is required for the {this.GetType().ToString()} component to function.");
            Id = RrStateService.GetPropertyIfIsNotNullElseIfNullSetToNewValueAndReturnNewValue(this, p => p.Id, Id);
            Visible = RrStateService.GetPropertyIfIsNotNullElseIfNullSetToNewValueAndReturnNewValue(this, p => p.Visible, Visible);
            Tag = RrStateService.GetPropertyIfIsNotNullElseIfNullSetToNewValueAndReturnNewValue(this, p => p.Tag, Tag);
            Placeholder = RrStateService.GetPropertyIfIsNotNullElseIfNullSetToNewValueAndReturnNewValue(this, p => p.Placeholder, Placeholder);
            DeBounce = RrStateService.GetPropertyIfIsNotNullElseIfNullSetToNewValueAndReturnNewValue(this, p => p.DeBounce, DeBounce);
            MultiLine = RrStateService.GetPropertyIfIsNotNullElseIfNullSetToNewValueAndReturnNewValue(this, p => p.MultiLine, MultiLine);
            inputText = RrStateService.GetPropertyIfIsNotNullElseIfNullSetToNewValueAndReturnNewValue(this, p => p.Text, Text);
            instantInputText = inputText;
        }

        protected override void OnInitialized()
        {
            RrStateService.RefreshAllComponents += StateHasChanged;
            RrStateService.RefreshSpecificComponentsById += (ids) => { if (ids is not null && ids.Contains(Id ?? "")) { StateHasChanged(); } };
            RrStateService.RefreshSpecificComponentsByTag += (tags) => { if (tags is not null && tags.Contains(Tag ?? "")) { StateHasChanged(); } };
        }

        private async void HandleOnKeyDown(KeyboardEventArgs e)
        {
            if (string.IsNullOrEmpty(inputText)) return;
            await OnKeyDown.InvokeAsync((e, inputText, Id!));
        }

        private async void OnTextChanged(ChangeEventArgs e)
        {
            string? newText = e.Value?.ToString();
            if (string.IsNullOrEmpty(newText)) return;
            if (DeBounce)
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
                RrStateService.SetComponentPropertyById<RrInput, string>(Id, c => c.Text, inputText);
                await OnChangedText.InvokeAsync(inputText);
            }
            else
            {
                inputText = newText;
                RrStateService.SetComponentPropertyById<RrInput, string>(Id, c => c.Text, inputText);
                await OnChangedText.InvokeAsync(inputText);
            }
            
        }
    }
}
