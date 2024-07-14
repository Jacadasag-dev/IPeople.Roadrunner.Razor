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

        [Parameter]
        public UIStates State { get; set; }

        [CascadingParameter]
        public string? LatchingPanelInitialSize { get; set; }

        [CascadingParameter]
        public int LatchingPanelsMininmumAdjustmentSize { get; set; }

        [CascadingParameter]
        public bool Latching { get; set; }

        [CascadingParameter]
        public LatchingTypes LatchingType { get; set; }
        #endregion

        #region Private Fields
        private string? panelWidth;
        private string? panelHeight;
        private string? panelTop;
        private string? panelLeft;
        private string? panelRight;
        private string? panelBottom;
        private string? stateChangerWidth;
        private string? stateChangerHeight;
        private string? stateChangerPosition;
        private string? stateChangerRight;
        private string? panelStateCssClass;
        private string? panelBodyOffsetHeight;
        private string? focusedZIndexStyling;
        private string? adjustedLatchedPanelSize;

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
            LatchingPanelInitialSize = RrStateService.GetPropertyIfIsNotNullElseIfNullSetToNewValueAndReturnNewValue(this, p => p.LatchingPanelInitialSize, LatchingPanelInitialSize);
            LatchingPanelsMininmumAdjustmentSize = RrStateService.GetPropertyIfIsNotNullElseIfNullSetToNewValueAndReturnNewValue(this, p => p.LatchingPanelsMininmumAdjustmentSize, LatchingPanelsMininmumAdjustmentSize);
            State = RrStateService.GetPropertyIfIsNotNullElseIfNullSetToNewValueAndReturnNewValue(this, p => p.State, State);
            // PanelHeightOffset
            panelBodyOffsetHeight = GetBodySizeOffset();
            // Set DotNetReference
            dotNetReference = DotNetObjectReference.Create(this);
            InitializePanelPositioningAndDimensions(PType);
        }

        private void InitializePanelPositioningAndDimensions(PanelTypes typeOfPanel)
        {
            if (RrStateService.AppGlobalVariables.BodyBounds is null)
                return;

            if (typeOfPanel == PanelTypes.Bottom || typeOfPanel == PanelTypes.Top)
            {
                if (PType == PanelTypes.Bottom)
                {
                    if (State == UIStates.Expanded)
                        panelBottom = Size;

                    if (State == UIStates.Collapsed)
                        panelBottom = "0px";
                }

                if (PType == PanelTypes.Top)
                {
                    if (State    == UIStates.Expanded)
                        panelTop = $"0px";

                    if (State == UIStates.Collapsed)
                        panelTop = $"-{Size}";
                }
            }
            else if (typeOfPanel == PanelTypes.Left || typeOfPanel == PanelTypes.Right)
            {
                if (PType == PanelTypes.Left)
                {
                    if (State == UIStates.Expanded)
                        panelLeft = $"{RrStateService.AppGlobalVariables.BodyBounds.LeftPosition}";

                    if (State == UIStates.Collapsed)
                        panelLeft = $"-{Size}";
                }
                if (PType == PanelTypes.Right)
                {
                    if (State == UIStates.Expanded)
                        panelRight = Size;

                    if (State == UIStates.Collapsed)
                        panelRight = $"0px";
                }
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

        private void HandleStateChangerClicked()
        {
            TogglePanelState();  
        }

        private void TogglePanelState()
        {
            if (State == Models.UIStates.Expanded)
            {
                RrStateService.SetComponentPropertyById<RrPanel, UIStates>(Id, s=> s.State, Models.UIStates.Collapsed);
            }
            else if (State == Models.UIStates.Collapsed)
            {
                RrStateService.SetComponentPropertyById<RrPanel, UIStates>(Id, s => s.State, State = Models.UIStates.Expanded);
            }
            StateHasChanged();
        }

        private void CollapsePanelState()
        {
            if (!Latching)
            {
                if (State == Models.UIStates.Expanded)
                {
                    RrStateService.SetComponentPropertyById<RrPanel, UIStates>(Id, s => s.State, Models.UIStates.Collapsed);
                }
                StateHasChanged();
            }
        }

        private string GetStateCssClass(Models.UIStates currentState)
        {
            if (Latching && LatchingType == LatchingTypes.Vertical) return "latching-vertical";
            if (Latching && LatchingType == LatchingTypes.Horizontal) return "latching-horizontal";
            if (currentState == Models.UIStates.Expanded)
            {
                return "expanded";
            }
            else if (currentState == Models.UIStates.Collapsed)
            {
                return "minimized";
            }
            else if (currentState == Models.UIStates.Neutral)
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

        [JSInvokable]
        public void FinishedDragging(int newSize)
        {
            string newPanelSize;
            if (newSize >= 0)
            {
                if (newSize < 100)
                {
                    newPanelSize = Size ?? "350px";
                    TogglePanelState();
                }
                else
                {
                    newPanelSize = $"{newSize}px";

                }
                RrStateService.SetComponentPropertyById<RrPanel, string>(Id, s => s.Size, newPanelSize);
            }
            StateHasChanged();
        }

        [JSInvokable]
        public void PanelClickedOnScriptHandler(string id, string action)
        {
            if (action == "latching-collapse" && id != Id)
                CollapsePanelState();
            
            if (action == "panel-focused")
            {
                focusedZIndexStyling = id == Id ? "z-index: 25" : "";
                StateHasChanged();
            }

        }

        private string? GetFocusedZIndexStyling()
        {
            return focusedZIndexStyling;
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
