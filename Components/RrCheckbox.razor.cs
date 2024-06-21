using Microsoft.AspNetCore.Components;

namespace IPeople.Roadrunner.Razor.Components
{
    public partial class RrCheckbox
    {
        [Parameter]
        public string Text { get; set; }

        [Parameter]
        public EventCallback OnCheckboxToggled { get; set; }
    }
}
