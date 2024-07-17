using IPeople.Roadrunner.Razor.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace IPeople.Roadrunner.Razor.Components
{
    public partial class RrPanel : IRrComponentBase
    {        
        #region Parameters
        [Parameter]
        public string? Id { get; set; }

        [Parameter]
        public string? Tag { get; set; }

        [Parameter]
        public string? Size { get; set; } = "400px";

        [Parameter]
        public PanelTypes PType { get; set; }

        [Parameter]
        public string? Style { get; set; }

        [Parameter]
        public RenderFragment? Header { get; set; }

        [Parameter]
        public RenderFragment? Body { get; set; }

        [Parameter]
        public RenderFragment? Footer { get; set; }

        [Parameter]
        public string? PeakingDistance { get; set; } = "100px";

        [Parameter]
        public SidePanelOffsets SidePanelOffset { get; set; } = SidePanelOffsets.None;

        [Parameter]
        public bool AllowScrollingX { get; set; } = false;

        [Parameter]
        public bool AllowScrollingY { get; set; } = false;

        [Parameter]
        public bool AllowScrolling { get; set; } = false;

        [Parameter]
        public bool Visible { get; set; } = true;

        [Parameter]
        public bool AllowLoading { get; set; } = false;

        [Parameter] 
        public UIStates State { get; set; } = UIStates.Collapsed;

        [Parameter]
        public bool HeaderDefaultStyling { get; set; } = false;

        [CascadingParameter]
        public LatchingTypes LatchingType { get; set; }

        [CascadingParameter(Name = "PageId")]
        public string? PageId { get; set; }
        #endregion

        public DotNetObjectReference<RrPanel>? dotNetReference;

        private bool afterFirstRender = false;

        private async void InitializePanel()
        {
            if (string.IsNullOrEmpty(Id)) throw new Exception($"The \"Id\" parameter must be defined and set as it is required for the {this.GetType().ToString()} component to function.");
            Id = RrStateService.GetPropertyIfIsNotNullElseIfNullSetToNewValueAndReturnNewValue(this, p => p.Id, Id);
            Tag = RrStateService.GetPropertyIfIsNotNullElseIfNullSetToNewValueAndReturnNewValue(this, p => p.Tag, Tag);
            Visible = RrStateService.GetPropertyIfIsNotNullElseIfNullSetToNewValueAndReturnNewValue(this, p => p.Visible, Visible);
            Size = RrStateService.GetPropertyIfIsNotNullElseIfNullSetToNewValueAndReturnNewValue(this, p => p.Size, Size);
            PType = RrStateService.GetPropertyIfIsNotNullElseIfNullSetToNewValueAndReturnNewValue(this, p => p.PType, PType);
            LatchingType = RrStateService.GetPropertyIfIsNotNullElseIfNullSetToNewValueAndReturnNewValue(this, p => p.LatchingType, LatchingType);
            State = RrStateService.GetPropertyIfIsNotNullElseIfNullSetToNewValueAndReturnNewValue(this, p => p.State, State);
            await SetPannelState(State);
            // Set DotNetReference
            dotNetReference = DotNetObjectReference.Create(this);
        }

        protected override void OnAfterRender(bool firstRender)
        {
            afterFirstRender = true;
        }

        private async Task SetPannelState(UIStates state)
        {
            if (afterFirstRender)
                await JS.InvokeVoidAsync("setPanelUIState", PageId, Id, state.ToString());
        }

        [JSInvokable]
        public void UpdateStateServicePanelState(string state)
        {
            if (state == "Expanded")
            {
                RrStateService.SetComponentPropertyById<RrPanel, UIStates>(Id, p => p.State, UIStates.Expanded);
            }
            else if (state == "Collapsed")
            {
                RrStateService.SetComponentPropertyById<RrPanel, UIStates>(Id, p => p.State, UIStates.Collapsed);
            }
        }

        private string GetBodySizeOffset()
        {
            if (Header is null && Footer is null)
            {
                return "0px";
            }
            else if (Header is not null && Footer is not null)
            {
                return "70px";
            }
            else
            {
                return "35px";
            }
        }
        private string GetInitialStateCssClass()
        {
            if (LatchingType == LatchingTypes.Vertical && (PType == PanelTypes.Left || PType == PanelTypes.Right)) return "latching-vertical";
            if (LatchingType == LatchingTypes.Horizontal && (PType == PanelTypes.Top || PType == PanelTypes.Bottom)) return "latching-horizontal";
            if (State == Models.UIStates.Expanded)
            {
                return "expanded";
            }
            else if (State == Models.UIStates.Collapsed)
            {
                return "minimized";
            }
            else if (State == Models.UIStates.Neutral)
            {
                return "";
            }
            return "invalid-state";
        }
        private string GetPanelTypeCssClass()
        {
            if (PType == PanelTypes.Left)
            {
                return "left";
            }
            else if (PType == PanelTypes.Right)
            {
                return "right";
            }
            else if (PType == PanelTypes.Top)
            {
                return "top";
            }
            else if (PType == PanelTypes.Bottom)
            {
                return "bottom";
            }
            return "invalid-type";
        }
        private string GetCenterStateChangerCssClass()
        {

            if (PType == PanelTypes.Left)
            {
                return "center-left";
            }
            else if (PType == PanelTypes.Right)
            {
                return "center-right";
            }
            return "";
        }
        private string GetAllowedScrolling()
        {
            if (AllowScrolling)
            {
                return "overflow: auto";
            }
            else
            {
                if (AllowScrollingX)
                {
                    return "overflow-x: auto";
                }
                if (AllowScrollingY)
                {
                    return "overflow-y: auto";
                }
            }
            return "";
        }
        private string GetHeaderStylingCssClass()
        {
            if (HeaderDefaultStyling)
            {
                return "Rr-panel-header-styling";
            }
            return "";
        }
    }
}
