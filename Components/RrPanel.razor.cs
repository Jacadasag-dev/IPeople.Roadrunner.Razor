using IPeople.Roadrunner.Razor.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using Radzen;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using System.Security.Principal;
using static System.Net.Mime.MediaTypeNames;

namespace IPeople.Roadrunner.Razor.Components
{
    public partial class RrPanel
    {
        #region Parameters
        [Parameter]
        public string? Id { get; set; }

        [Parameter]
        public string? Tag { get; set; }

        [Parameter]
        public string? PanelSize { get; set; } = "400px";

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
        public Models.RrPanel? Panel { get; set; }

        [CascadingParameter]
        public string? LatchingPanels { get; set; }

        [CascadingParameter]
        public int MinLatchingPanelWidth { get; set; }
        #endregion

        #region Private Fields
        private Models.RrPanel? panelFromService;
        private PanelTypes panelType;
        private string? exceptionMessage;
        private UIStates panelUIState;
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
        private string? initialPanelSize;
        private string? focusedZIndexStyling;
        private bool isLatching = false;
        #endregion

        private void InitializePanel()
        {
            InitializePanelModel();
            InitializeFields();
        }

        private void InitializePanelModel()
        {
            if (!string.IsNullOrEmpty(Id))
            {
                panelFromService = RrStateService.GetComponentById<Models.RrPanel>(Id) as Models.RrPanel;
            }
            else if (Panel is not null)
            {
                panelFromService = RrStateService.GetComponent<Models.RrPanel>(Panel) as Models.RrPanel;
                if (panelFromService is not null)
                {
                    Id = panelFromService.Identifier;
                }
            }
            if (panelFromService is null)
            {
                if (!string.IsNullOrEmpty(Id))
                {
                    RrStateService.RegisterComponentById<Models.RrPanel>(Id);
                    panelFromService = RrStateService.GetComponentById<Models.RrPanel>(Id) as Models.RrPanel;
                    RrStateService.RefreshAllComponents += StateHasChanged;
                    RrStateService.RefreshSpecificComponentsById += (ids) => { if (ids is not null && ids.Contains(Id)) StateHasChanged(); };
                    if (panelFromService is not null)
                        RrStateService.RefreshSpecificComponentsByTag += (tags) => { if (tags is not null && tags.Contains(Tag ?? panelFromService.Tag ?? string.Empty)) StateHasChanged(); };
                }
                else
                {
                    exceptionMessage = "ERROR:id_is_required";
                }
            }
            if (panelFromService is not null)
            {
                if (string.IsNullOrEmpty(panelFromService?.Tag))
                {
                    if (!string.IsNullOrEmpty(Tag))
                    {
                        RrStateService.SetComponentProperty<Models.RrPanel, string>(panelFromService, c => c.Tag, Tag);
                    }
                }
            }
        }

        private void InitializeFields()
        {
            // PanelType
            panelType = panelFromService is not null ? panelFromService.Type ?? PType : PType;
            // Latching
            isLatching = !string.IsNullOrEmpty(LatchingPanels) && (panelType == PanelTypes.Left || panelType == PanelTypes.Right);
            if (panelFromService is not null)
            {
                if (panelFromService.State == UIStates.Neutral)
                {
                    panelUIState = InitialState;
                }
                else
                {
                    panelUIState = panelFromService.State;
                }
            }
            else
            {
                panelUIState = InitialState;
            }

            // PanelSize
            initialPanelSize = panelFromService is not null ? panelFromService.Size ?? PanelSize : PanelSize;

            // PanelStateCssClass
            panelStateCssClass = GetStateCssClass(panelUIState);

            // PanelPositioningAndDimensions
            InitializePanelPositioningAndDimensions(panelType);

            // PanelHeightOffset
            if (Header is null && Footer is null)
            {
                panelBodyOffsetHeight = "0px";
            }
            else if (Header is not null && Footer is not null)
            {
                panelBodyOffsetHeight = "70px";
            }
            else
            {
                panelBodyOffsetHeight = "35px";
            }
        }

        private void InitializePanelPositioningAndDimensions(PanelTypes typeOfPanel)
        {
            if (RrStateService.AppGlobalVariables.BodyBounds is null)
                return;

            if (typeOfPanel == PanelTypes.Bottom || typeOfPanel == PanelTypes.Top)
            {
                panelLeft = $"{RrStateService.AppGlobalVariables.BodyBounds.LeftPosition}px";
                panelWidth = $"{RrStateService.AppGlobalVariables.BodyBounds.Width}px";
                panelHeight = initialPanelSize;
                stateChangerWidth = panelWidth;
                stateChangerHeight = "10px";

                if (panelType == PanelTypes.Bottom)
                {
                    if (panelUIState == UIStates.Expanded)
                        panelBottom = initialPanelSize;

                    if (panelUIState == UIStates.Collapsed)
                        panelBottom = "0px";

                    stateChangerPosition = "0px";
                }

                if (panelType == PanelTypes.Top)
                {
                    if (panelUIState == UIStates.Expanded)
                        panelTop = $"0px";

                    if (panelUIState == UIStates.Collapsed)
                        panelTop = $"-{initialPanelSize}";

                    stateChangerPosition = $"{initialPanelSize}";
                }
            }
            else if (typeOfPanel == PanelTypes.Left || typeOfPanel == PanelTypes.Right)
            {
                int heightOffset = (SidePanelOffset == SidePanelOffsets.Both) ? 20 : 10;
                int topOffset = (SidePanelOffset == SidePanelOffsets.Top || SidePanelOffset == SidePanelOffsets.Both) ? 10 : 0;

                if (!isLatching)
                    panelWidth = initialPanelSize;      

                stateChangerWidth = "10px";
                stateChangerHeight = panelHeight;
                panelTop = $"{topOffset}px";
                panelHeight = (SidePanelOffset == SidePanelOffsets.Bottom || SidePanelOffset == SidePanelOffsets.Top || SidePanelOffset == SidePanelOffsets.Both)
                    ? $"calc({RrStateService.AppGlobalVariables.BodyBounds.Height}px - {heightOffset}px)"
                    : $"{RrStateService.AppGlobalVariables.BodyBounds.Height}px";

                if (panelType == PanelTypes.Left)
                {
                    if (isLatching)
                    {
                        panelLeft = $"{RrStateService.AppGlobalVariables.BodyBounds.LeftPosition}";
                        panelWidth = $"{LatchingPanels}";
                    }
                    else
                    {
                        if (panelUIState == UIStates.Expanded)
                            panelLeft = $"{RrStateService.AppGlobalVariables.BodyBounds.LeftPosition}";

                        if (panelUIState == UIStates.Collapsed)
                            panelLeft = $"-{initialPanelSize}";

                    }
                    stateChangerPosition = panelWidth;
                }
                if (panelType == PanelTypes.Right)
                {
                    if (isLatching)
                    {
                        panelRight = $"calc({RrStateService.AppGlobalVariables.BodyBounds.Width - 20}px - {LatchingPanels})";
                        panelWidth = $"calc({RrStateService.AppGlobalVariables.BodyBounds.Width - 20}px - {LatchingPanels})";
                    }
                    else
                    {
                        if (panelUIState == UIStates.Expanded)
                            panelRight = initialPanelSize;

                        if (panelUIState == UIStates.Collapsed)
                            panelRight = $"0px";
                    }
                    stateChangerPosition = "0px";
                }
            }
        }

        private void HandleStateChangerClicked()
        {
            TogglePanelState();  
        }

        private void TogglePanelState()
        {
            if (panelUIState == Models.UIStates.Expanded)
            {
                RrStateService.SetComponentProperty<Models.RrPanel, UIStates>(panelFromService, s=> s.State, Models.UIStates.Collapsed);
            }
            else if (panelUIState == Models.UIStates.Collapsed)
            {
                RrStateService.SetComponentProperty<Models.RrPanel, UIStates>(panelFromService, s => s.State, panelUIState = Models.UIStates.Expanded);
            }
            StateHasChanged();
        }

        private void CollapsePanelState()
        {
            if (panelUIState == Models.UIStates.Expanded)
            {
                RrStateService.SetComponentProperty<Models.RrPanel, UIStates>(panelFromService, s => s.State, Models.UIStates.Collapsed);
            }
            StateHasChanged();
        }

        private string GetStateCssClass(Models.UIStates currentState)
        {
            if (isLatching) return "neutral";
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
            if (panelType == PanelTypes.Left)
            {
                return "left";
            }
            else if (panelType == PanelTypes.Right)
            {
                return "right";
            }
            else if (panelType == PanelTypes.Top)
            {
                return "top";
            }
            else if (panelType == PanelTypes.Bottom)
            {
                return "bottom";
            }
            return "invalid-type";
        }

        private string GetCenterStateChangerCssClass()
        {

            if (panelType == PanelTypes.Left)
            {
                return "center-left";
            }
            else if (panelType == PanelTypes.Right)
            {
                return "center-right";
            }
            return "";
        }

        private string GetPanelLatchingCssClass()
        {
            if (isLatching)
            {
                return "latching";
            }
            return "";
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                await JS.InvokeVoidAsync("registerPanels", Id, DotNetObjectReference.Create(this), panelType.ToString(), isLatching, MinLatchingPanelWidth);
            }
            RrStateService.SetComponentProperty<Models.RrPanel, bool>(panelFromService, s => s.Transition, true);
        }

        [JSInvokable]
        public void FinishedDragging(int newSize)
        {
            string newPanelSize;
            if (newSize < 100)
            {
                newPanelSize = PanelSize ?? "350px";
                TogglePanelState();
            } else
            {
                newPanelSize = $"{newSize}px";
            }
            RrStateService.SetComponentProperty<Models.RrPanel, string>(panelFromService, s => s.Size, newPanelSize);
        }

        [JSInvokable]
        public void PanelClickedOnScriptHandler(string id, string action)
        {
            if (action == "latching-collapse" && id != Id)
                CollapsePanelState();
            
            if (action == "panel-focused")
                focusedZIndexStyling = id == Id ? "z-index: 25" : "";
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
