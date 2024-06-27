using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IPeople.Roadrunner.Razor.Components
{
    public partial class RrLoading
    {
        [Parameter]
        public string Style { get; set; } = "";
        [Parameter]
        public string Message { get; set; } = "Loading...";
    }
}
