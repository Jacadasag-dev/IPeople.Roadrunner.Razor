namespace IPeople.Roadrunner.Razor.Models
{
    public class TreeViewIconPaths
    {
        public string DPMIcon { get; set; } = "_content/IPeople.Roadrunner.Razor/images/MOIcons/dpmicon.png";
        public string AppIcon { get; set; } = "_content/IPeople.Roadrunner.Razor/images/MOIcons/appicon.png";
        public string TableIcon { get; set; } = "_content/IPeople.Roadrunner.Razor/images/MOIcons/tableicon.png";
        public string ScheduledIcon { get; set; } = "_content/IPeople.Roadrunner.Razor/images/MOIcons/schedule-16.png";
        public string PuzzlePieceIcon { get; set; } = "_content/IPeople.Roadrunner.Razor/images/MOIcons/puzzlepieceicon-16.png";
        public string KeyIcon { get; set; } = "_content/IPeople.Roadrunner.Razor/images/MOIcons/keyicon.png";
        public string BuildingIcon { get; set; } = "_content/IPeople.Roadrunner.Razor/images/StatusIcons/building-16.png";
        public string LiveIcon { get; set; } = "_content/IPeople.Roadrunner.Razor/images/StatusIcons/live-16.png";
        public string ErrorIcon { get; set; } = "_content/IPeople.Roadrunner.Razor/images/StatusIcons/error-14.png";
        public string OkayIcon { get; set; } = "_content/IPeople.Roadrunner.Razor/images/StatusIcons/okay-16.png";
        public string ValidatingIcon { get; set; } = "_content/IPeople.Roadrunner.Razor/images/StatusIcons/validating-16.png";
        public string UnknownIcon { get; set; } = "_content/IPeople.Roadrunner.Razor/images/StatusIcons/question-mark-16.png";
        // Add other icon paths as needed
    }

    public enum LatchingTypes
    {
        None,
        Vertical,
        Horizontal,
    }
    public enum ButtonSizes
    {
        Small,
        Medium,
        Large
    }

    public enum PanelTypes
    {
        Left,
        Right,
        Bottom,
        Top,
    }

    public enum SidePanelOffsets
    {
        Top,
        Bottom,
        Both,
        None
    }

    public enum RrLoadingType
    {
        Awaiting,
        Loading,
        Querying,
        Authenticating,
    }

    public enum UIStates
    {
        Expanded,
        Collapsed,
        Neutral,
        Peaking
    }
}
