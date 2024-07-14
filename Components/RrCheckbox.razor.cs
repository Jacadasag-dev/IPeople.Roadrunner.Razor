using IPeople.Roadrunner.Razor.Models;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IPeople.Roadrunner.Razor.Components
{
    public partial class RrCheckbox : IRrComponentBase
    {
        #region Parameters
        [Parameter]
        public string? Id { get; set; }
        [Parameter]
        public string Style { get; set; } = "";
        [Parameter]
        public string? Label { get; set; } = "";

        [Parameter]
        public string? Tag { get; set; } = "";

        [Parameter]
        public bool Visible { get; set; } = true;

        [Parameter]
        public EventCallback OnCheckboxClicked { get; set; }

        [Parameter] 
        public bool InitiallyChecked { get; set; } = false;

        [Parameter]
        public bool Checked { get; set; }

        #endregion

        #region Private Fields
        private bool isChecked;
        #endregion

        private void InitializeCheckbox()
        {
            if (string.IsNullOrEmpty(Id)) throw new Exception($"The \"Id\" parameter must be defined and set as it is required for the {this.GetType().ToString()} component to function.");
            Id = RrStateService.GetPropertyIfIsNotNullElseIfNullSetToNewValueAndReturnNewValue<RrCheckbox, string>(this, p => p.Id, Id);
            Tag = RrStateService.GetPropertyIfIsNotNullElseIfNullSetToNewValueAndReturnNewValue<RrCheckbox, string>(this, p => p.Tag, Tag);
            Visible = RrStateService.GetPropertyIfIsNotNullElseIfNullSetToNewValueAndReturnNewValue<RrCheckbox, bool>(this, p => p.Visible, Visible);
            Label = RrStateService.GetPropertyIfIsNotNullElseIfNullSetToNewValueAndReturnNewValue<RrCheckbox, string>(this, p => p.Label, Label);
            InitiallyChecked = RrStateService.GetPropertyIfIsNotNullElseIfNullSetToNewValueAndReturnNewValue<RrCheckbox, bool>(this, p => p.InitiallyChecked, InitiallyChecked);
            isChecked = RrStateService.GetPropertyIfIsNotNullElseIfNullSetToNewValueAndReturnNewValue<RrCheckbox, bool> (this, p => p.Checked, isChecked);
        }

        private void HandleOnCheckboxClick()
        {
            RrStateService.SetComponentPropertyById<RrCheckbox, bool>(Id, p => p.Checked, !isChecked);
            OnCheckboxClicked.InvokeAsync();
        }
    }
}
