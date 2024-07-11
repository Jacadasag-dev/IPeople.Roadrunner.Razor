using IPeople.Roadrunner.Razor.Models;
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
        #region Parameters
        [Parameter]
        public string? Id { get; set; }
        [Parameter]
        public string Style { get; set; } = "";
        [Parameter]
        public string Label { get; set; } = "";

        [Parameter]
        public string Tag { get; set; } = "";

        [Parameter]
        public bool Visible { get; set; } = true;

        [Parameter]
        public EventCallback OnCheckboxClicked { get; set; }

        [Parameter] 
        public bool InitiallyChecked { get; set; } = false;

        [Parameter]
        public Models.RrCheckbox? Checkbox { get; set; }
        #endregion

        #region Private Fields
        private Models.RrCheckbox? checkboxFromService;
        private bool visible;
        private string? label;
        private string? exceptionMessage;
        private bool isChecked;
        #endregion

        private void InitializeCheckbox()
        {
            if (!string.IsNullOrEmpty(Id))
            {
                checkboxFromService = RrStateService.GetComponentById<Models.RrCheckbox>(Id);
            }
            else if (Checkbox is not null)
            {
                checkboxFromService = RrStateService.GetComponent<Models.RrCheckbox>(Checkbox);
                if (checkboxFromService is not null)
                {
                    Id = checkboxFromService.Identifier;
                }
            }
            if (checkboxFromService is null)
            {
                if (!string.IsNullOrEmpty(Id))
                {
                    RrStateService.RegisterComponentById<Models.RrCheckbox>(Id);
                    checkboxFromService = RrStateService.GetComponentById<Models.RrCheckbox>(Id);
                    RrStateService.RefreshAllComponents += StateHasChanged;
                    RrStateService.RefreshSpecificComponentsById += (ids) => { if (ids is not null && ids.Contains(Id)) StateHasChanged(); };
                    if (checkboxFromService is not null)
                        RrStateService.RefreshSpecificComponentsByTag += (tags) => { if (tags is not null && tags.Contains(Tag ?? checkboxFromService.Tag ?? string.Empty)) StateHasChanged(); };

                }
                else
                {
                    exceptionMessage = "ERROR:id_is_required";
                }
            }
            visible = checkboxFromService is not null ? checkboxFromService?.Visible ?? Visible : Visible;
            label = checkboxFromService is not null ? checkboxFromService?.Text ?? Label : Label;
            isChecked = checkboxFromService is not null ? checkboxFromService?.IsChecked ?? InitiallyChecked : InitiallyChecked;
        }

        private void HandleOnCheckboxClick()
        {
            RrStateService.SetComponentProperty<Models.RrCheckbox, bool>(checkboxFromService, s => s.IsChecked, !isChecked);
            OnCheckboxClicked.InvokeAsync();
        }
    }
}
