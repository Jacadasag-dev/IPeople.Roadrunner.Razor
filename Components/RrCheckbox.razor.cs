using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IPeople.Roadrunner.Razor.Components
{
    public partial class RrCheckbox
    {
        [Parameter]
        public string Style { get; set; } = "";
        [Parameter]
        public string Label { get; set; } = "";

        [Parameter]
        public string Tag { get; set; } = "";

        [Parameter]
        public EventCallback OnCheckboxToggled { get; set; }
    }
}
