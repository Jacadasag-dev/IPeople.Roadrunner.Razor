using IPeople.Roadrunner.Razor.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using Radzen;
using System.Reflection.Emit;
using System.Security.Principal;
using static System.Net.Mime.MediaTypeNames;

namespace IPeople.Roadrunner.Razor.Components
{
    public partial class RrPanel
    {
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
        public bool Visible { get; set; } = true;

        [Parameter]
        public Models.RrPanel? Panel { get; set; }

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
        private string? panelSize;

        private void InitializePanel()
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
                else {
                    exceptionMessage = "ERROR:id_is_required";
                }
            }
            bool notNull = panelFromService is not null;
            if (notNull)
            {
                if (string.IsNullOrEmpty(panelFromService?.Tag))
                {
                    if (!string.IsNullOrEmpty(Tag))
                    {
                        RrStateService.SetComponentProperty<Models.RrPanel, string>(panelFromService, c => c.Tag, Tag);
                    }
                }
            }
            panelType = notNull && panelFromService is not null ? panelFromService.Type ?? PType : PType;
            panelUIState = notNull && panelFromService is not null ? panelFromService.State : UIStates.Expanded;
            panelSize = notNull && panelFromService is not null ? panelFromService.Size ?? PanelSize : PanelSize;

            panelStateCssClass = GetStateCssClass(panelUIState);

            if (RrStateService.AppGlobalVariables.BodyBounds is not null)
            {
                if (panelType == PanelTypes.Bottom)
                {
                    if (panelUIState == UIStates.Expanded)
                        panelBottom = panelSize;
                    
                    if (panelUIState == UIStates.Collapsed)
                        panelBottom = "0px";

                    panelLeft = $"{RrStateService.AppGlobalVariables.BodyBounds.LeftPosition}px";
                    panelWidth = $"{RrStateService.AppGlobalVariables.BodyBounds.Width}px";
                    panelHeight = panelSize;
                    stateChangerPosition = "0px";
                    stateChangerWidth = panelWidth;
                    stateChangerHeight = "10px";
                }

                if (panelType == PanelTypes.Top)
                {
                    if (panelUIState == UIStates.Expanded)
                        panelTop = $"0px";

                    if (panelUIState == UIStates.Collapsed)
                        panelTop = $"-{panelSize}";

                    
                    panelLeft = $"{RrStateService.AppGlobalVariables.BodyBounds.LeftPosition}px";
                    panelWidth = $"{RrStateService.AppGlobalVariables.BodyBounds.Width}px";
                    panelHeight = panelSize;
                    stateChangerWidth = panelWidth;
                    stateChangerHeight = "10px";
                    stateChangerPosition = $"{panelSize}";
                }

                if (panelType == PanelTypes.Left)
                {
                    if (panelUIState == UIStates.Expanded)
                        panelLeft = $"{RrStateService.AppGlobalVariables.BodyBounds.LeftPosition}";

                    if (panelUIState == UIStates.Collapsed)
                        panelLeft = $"-{panelSize}";

                    panelWidth = panelSize;
                    panelHeight = $"{RrStateService.AppGlobalVariables.BodyBounds.Height}px";

                    var heightOffset = (SidePanelOffset == SidePanelOffsets.Both) ? 20 : 10;
                    var topOffset = (SidePanelOffset == SidePanelOffsets.Top || SidePanelOffset == SidePanelOffsets.Both) ? 10 : 0;
                    if (SidePanelOffset == SidePanelOffsets.Bottom || SidePanelOffset == SidePanelOffsets.Top || SidePanelOffset == SidePanelOffsets.Both)
                    {
                        panelHeight = $"calc({RrStateService.AppGlobalVariables.BodyBounds.Height}px - {heightOffset}px)";
                    }
                    else
                    {
                        panelHeight = $"{RrStateService.AppGlobalVariables.BodyBounds.Height}px";
                    }
                    panelTop = $"{topOffset}px";
                    stateChangerHeight = panelHeight;


                    stateChangerWidth = "10px";
                    stateChangerPosition = panelWidth;
                }
                if (panelType == PanelTypes.Right)
                {
                    if (panelUIState == UIStates.Expanded)
                        panelRight = panelSize;

                    if (panelUIState == UIStates.Collapsed)
                        panelRight = $"0px";

                    panelWidth = panelSize;

                    var heightOffset = (SidePanelOffset == SidePanelOffsets.Both) ? 20 : 10;
                    var topOffset = (SidePanelOffset == SidePanelOffsets.Top || SidePanelOffset == SidePanelOffsets.Both) ? 10 : 0;
                    if (SidePanelOffset == SidePanelOffsets.Bottom || SidePanelOffset == SidePanelOffsets.Top || SidePanelOffset == SidePanelOffsets.Both)
                    {
                        panelHeight = $"calc({RrStateService.AppGlobalVariables.BodyBounds.Height}px - {heightOffset}px)";
                    }
                    else
                    {
                        panelHeight = $"{RrStateService.AppGlobalVariables.BodyBounds.Height}px";
                    }
                    panelTop = $"{topOffset}px";
                    stateChangerHeight = panelHeight;
                    stateChangerPosition = "0px";
                    stateChangerWidth = "10px";
                }
            }

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
        }

        private string GetStateCssClass(Models.UIStates currentState)
        {
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

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                await JS.InvokeVoidAsync("registerPanels", Id, DotNetObjectReference.Create(this), PType.ToString());
            }
            RrStateService.SetComponentProperty<Models.RrPanel, bool>(panelFromService, s => s.Transition, true);
        }


        [JSInvokable]
        public void FinishedDragging(int newSize)
        {
            if (newSize < 100)
                newSize = 100;

            RrStateService.SetComponentProperty<Models.RrPanel, string>(panelFromService, s => s.Size, $"{newSize}px");
        }
    }
}
