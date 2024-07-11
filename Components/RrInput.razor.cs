using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components;
using IPeople.Roadrunner.Razor.Models;

namespace IPeople.Roadrunner.Razor.Components
{
    public partial class RrInput
    {
        #region Parameters
        [Parameter]
        public string? Id { get; set; }

        [Parameter]
        public string? Tag { get; set; }

        [Parameter]
        public bool Visible { get; set; } = true;

        [Parameter]
        public string Placeholder { get; set; } = "Start typing...";

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

        [Parameter]
        public Models.RrInput? Input { get; set; }
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
            if (!string.IsNullOrEmpty(Id))
            {
                inputFromService = RrStateService.GetComponentById<Models.RrInput>(Id) as Models.RrInput;
            }
            else if (Input is not null)
            {
                inputFromService = RrStateService.GetComponent<Models.RrInput>(Input) as Models.RrInput;
                if (inputFromService is not null)
                {
                    Id = inputFromService.Identifier;
                }
            }
            if (inputFromService is null)
            {
                if (!string.IsNullOrEmpty(Id))
                {
                    RrStateService.RegisterComponentById<Models.RrInput>(Id);
                    inputFromService = RrStateService.GetComponentById<Models.RrInput>(Id) as Models.RrInput;
                    RrStateService.RefreshAllComponents += StateHasChanged;
                    RrStateService.RefreshSpecificComponentsById += (ids) => { if (ids is not null && ids.Contains(Id)) StateHasChanged(); };
                    if (inputFromService is not null)
                        RrStateService.RefreshSpecificComponentsByTag += (tags) => { if (tags is not null && tags.Contains(Tag ?? inputFromService.Tag ?? string.Empty)) StateHasChanged(); };

                }
                else
                {
                    exceptionMessage = "ERROR:id_is_required";
                }
            }
            bool notNull = inputFromService is not null;
            if (notNull)
            {
                if (string.IsNullOrEmpty(inputFromService?.Tag))
                {
                    if (!string.IsNullOrEmpty(Tag))
                    {
                        RrStateService.SetComponentProperty<Models.RrInput, string>(inputFromService, c => c.Tag, Tag);
                    }
                }
            }
            visible = notNull ? inputFromService?.Visible ?? Visible : Visible;
            placeholder = notNull ? inputFromService?.PlaceHolder ?? Placeholder : Placeholder;
            deBounce = notNull ? inputFromService?.DoDeBounce ?? DeBounce : DeBounce;
            inputText = notNull ? string.IsNullOrEmpty(inputFromService?.Text) ? Text : (string.IsNullOrEmpty(Text) ? inputFromService.Text : Text) : exceptionMessage;
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
                RrStateService.SetComponentProperty<Models.RrInput, string>(inputFromService, c => c.Text, inputText);
                await OnChangedText.InvokeAsync(inputText);
            }
            else
            {
                inputText = newText;
                RrStateService.SetComponentProperty<Models.RrInput, string>(inputFromService, c => c.Text, inputText);
                await OnChangedText.InvokeAsync(inputText);
            }
        }
    }
}
