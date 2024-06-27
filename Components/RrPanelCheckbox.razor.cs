using Microsoft.AspNetCore.Components;

namespace IPeople.Roadrunner.Razor.Components
{
    public partial class RrPanelCheckbox
    {
        [Parameter]
        public string Style { get; set; } = "";

        [Parameter]
        public Models.RrPanel Panel { get; set; }

        [Parameter]
        public EventCallback OnCheckboxToggled { get; set; }
    }
}
