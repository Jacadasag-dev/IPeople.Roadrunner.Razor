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
        public UIStates InitialState { get; set; } = UIStates.Expanded;

        [Parameter]
        public bool HeaderDefaultStyling { get; set; } = false;

        [CascadingParameter]
        public bool Latching { get; set; }

        [CascadingParameter]
        public LatchingTypes LatchingType { get; set; }
        #endregion

        public DotNetObjectReference<RrPanel>? dotNetReference;

        private void InitializePanel()
        {
            if (string.IsNullOrEmpty(Id)) throw new Exception($"The \"Id\" parameter must be defined and set as it is required for the {this.GetType().ToString()} component to function.");
            Id = RrStateService.GetPropertyIfIsNotNullElseIfNullSetToNewValueAndReturnNewValue(this, p => p.Id, Id);
            Tag = RrStateService.GetPropertyIfIsNotNullElseIfNullSetToNewValueAndReturnNewValue(this, p => p.Tag, Tag);
            Visible = RrStateService.GetPropertyIfIsNotNullElseIfNullSetToNewValueAndReturnNewValue(this, p => p.Visible, Visible);
            Size = RrStateService.GetPropertyIfIsNotNullElseIfNullSetToNewValueAndReturnNewValue(this, p => p.Size, Size);
            PType = RrStateService.GetPropertyIfIsNotNullElseIfNullSetToNewValueAndReturnNewValue(this, p => p.PType, PType);
            Latching = RrStateService.GetPropertyIfIsNotNullElseIfNullSetToNewValueAndReturnNewValue(this, p => p.Latching, Latching);
            LatchingType = RrStateService.GetPropertyIfIsNotNullElseIfNullSetToNewValueAndReturnNewValue(this, p => p.LatchingType, LatchingType);
            InitialState = RrStateService.GetPropertyIfIsNotNullElseIfNullSetToNewValueAndReturnNewValue(this, p => p.InitialState, InitialState);
            // Set DotNetReference
            dotNetReference = DotNetObjectReference.Create(this);
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
            if (Latching && LatchingType == LatchingTypes.Vertical) return "latching-vertical";
            if (Latching && LatchingType == LatchingTypes.Horizontal) return "latching-horizontal";
            if (InitialState == Models.UIStates.Expanded)
            {
                return "expanded";
            }
            else if (InitialState == Models.UIStates.Collapsed)
            {
                return "minimized";
            }
            else if (InitialState == Models.UIStates.Neutral)
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
