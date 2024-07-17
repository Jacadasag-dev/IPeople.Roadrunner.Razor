using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace IPeople.Roadrunner.Razor.Components
{
    public partial class RrLoading
    {
        #region Parameters
        [Parameter]
        public string? Id { get; set; }

        [Parameter]
        public string Style { get; set; } = "";

        [Parameter]
        public bool FullScreen { get; set; } = false;

        [Parameter]
        public Models.RrLoadingBase? Loading { get; set; }

        [Parameter]
        public EventCallback<Models.RrLoadingBase> LoadingChange { get; set; }
        #endregion

        protected override async Task OnParametersSetAsync()
        {
            await LoadingChange.InvokeAsync(Loading);
        }
    }
}
