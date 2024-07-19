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
        public UIStates State { get; set; }

        [Parameter]
        public bool HeaderDefaultStyling { get; set; } = false;

        [Parameter]
        public UIStates InitialState { get; set; }

        [CascadingParameter]
        public bool LatchingDetachable { get; set; }

        [CascadingParameter]
        public LatchingTypes LatchingType { get; set; }

        [CascadingParameter]
        public bool PanelLatching { get; set; }

        [CascadingParameter(Name = "PageId")]
        public string? PageId { get; set; }
        #endregion

        public DotNetObjectReference<RrPanel>? dotNetReference;

        private bool afterFirstRender = false;
        private RrLoadingBase? loading;

        private async void InitializePanel()
        {
            if (string.IsNullOrEmpty(Id)) throw new Exception($"The \"Id\" parameter must be defined and set as it is required for the {this.GetType().ToString()} component to function.");
            Id = RrStateService.GetPropertyIfIsNotNullElseIfNullSetToNewValueAndReturnNewValue(this, p => p.Id, Id);
            Tag = RrStateService.GetPropertyIfIsNotNullElseIfNullSetToNewValueAndReturnNewValue(this, p => p.Tag, Tag);
            Visible = RrStateService.GetPropertyIfIsNotNullElseIfNullSetToNewValueAndReturnNewValue(this, p => p.Visible, Visible);
            Size = RrStateService.GetPropertyIfIsNotNullElseIfNullSetToNewValueAndReturnNewValue(this, p => p.Size, Size);
            PType = RrStateService.GetPropertyIfIsNotNullElseIfNullSetToNewValueAndReturnNewValue(this, p => p.PType, PType);
            State = RrStateService.GetPropertyIfIsNotNullElseIfNullSetToNewValueAndReturnNewValue(this, p => p.State, State);
            if (State == UIStates.Neutral)
                State = InitialState;

            await SetPannelState();
            // Set DotNetReference
            dotNetReference = DotNetObjectReference.Create(this);
        }
        protected override void OnInitialized()
        {
            RrStateService.RefreshAllComponents += StateHasChanged;
            RrStateService.RefreshSpecificComponentsById += (ids) => { if (ids is not null && ids.Contains(Id ?? "")) { StateHasChanged(); } };
            RrStateService.RefreshSpecificComponentsByTag += (tags) => { if (tags is not null && tags.Contains(Tag ?? "")) { StateHasChanged(); } };
            RrStateService.LoadingStateChangeRequestById += (id , newLoading) => { if (id == Id) { loading = newLoading; StateHasChanged(); } };
            RrStateService.LoadingStateChangeRequestByTag += (tag, newLoading) => { if (tag == Tag) { loading = newLoading; StateHasChanged(); } };
            RrStateService.StopAllLoading += () => { if (loading is not null && loading.IsLoading) { loading = new(); StateHasChanged(); } };
        }

        protected override void OnAfterRender(bool firstRender)
        {
            afterFirstRender = true;
        }

        private async Task SetPannelState()
        {
            if (afterFirstRender)
                await JS.InvokeVoidAsync("setPanelUIState", PageId, Id, State.ToString());
        }

        [JSInvokable]
        public void UpdateStateServicePanelState(string state)
        {
            if (state == "Expanded")
            {
                RrStateService.SetComponentPropertyById<RrPanel, UIStates>(Id, p => p.State, UIStates.Expanded);
                State = UIStates.Expanded;
            }
            else if (state == "Collapsed")
            {
                RrStateService.SetComponentPropertyById<RrPanel, UIStates>(Id, p => p.State, UIStates.Collapsed);
                State = UIStates.Collapsed;
            }
        }

        [JSInvokable]
        public void UpdatePanelLatching(bool latching)
        {
            RrStateService.SetComponentPropertyById<RrPanel, bool>(PageId, p => p.PanelLatching, latching);
            PanelLatching = latching;
            StateHasChanged();
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
            if (PanelLatching && LatchingType == LatchingTypes.Vertical && (PType == PanelTypes.Left || PType == PanelTypes.Right)) return "latching-vertical";
            if (PanelLatching && LatchingType == LatchingTypes.Horizontal && (PType == PanelTypes.Top || PType == PanelTypes.Bottom)) return "latching-horizontal";
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
