using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IPeople.Roadrunner.Razor.Components
{
    public partial class RrPage
    {
        [Parameter]
        public RenderFragment Header { get; set; }

        [Parameter]
        public RenderFragment Body { get; set; }

        [Parameter]
        public RenderFragment Footer { get; set; }

        [Parameter]
        public string TopOffset { get; set; } = "0px";

        [Parameter]
        public string LeftOffset { get; set; } = "0px";
    }
}
