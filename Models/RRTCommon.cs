
using Microsoft.AspNetCore.Components;

namespace IPeople.Roadrunner.Razor.Models
{
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

    public class PageBodyBounds
    {
        public int TopPosition { get; set; }
        public int LeftPosition { get; set; }
        public int RightPosition { get; set; }
        public int BottomPosition { get; set; }
        public int Height { get; set; }
        public int Width { get; set; }

    }

    public class RrLoading
    {
        public RrLoadingType Type { get; set; } = RrLoadingType.Authenticating;
        public bool IsLoading { get; set; } = true;
        public string Message { get; set; } = "Authenticating...";
    }

    public enum RrLoadingType
    {
        Awaiting,
        Loading,
        Querying,
        Authenticating,
    }

    /// <summary>
    /// Used by IPeople.Roadrunner.Razor for custom razor component for RrScout
    /// </summary>
    public interface IRrComponentBase
    {
        public string Identifier { get; set; }
        public bool Visible { get; set; }
        public string Tag { get; set; }
    }

    public interface IRrComponentWithData<T>
    {
        T Data { get; set; }
    }

    /// <summary>
    /// Used by IPeople.Roadrunner.Razor for custom razor component for RrScout
    /// </summary>
    /// 

    public class RrInput : IRrComponentBase
    {
        public string Identifier { get; set; }
        public bool Visible { get; set; }
        public string Text { get; set; }
        public string Tag { get; set; }
        public string PlaceHolder { get; set; }
        public bool DoDeBounce { get; set; } = true;
        public RrInput(string id)
        {
            Identifier = id;
        }
    }


    /// <summary>
    /// Used by IPeople.Roadrunner.Razor for custom razor component for RrScout
    /// </summary>
    public class RrDropdown : IRrComponentBase
    {
        public string Identifier { get; set; }
        public bool Visible { get; set; }
        public string? Tag { get; set; }
        public string? Label { get; set; }
        public object? SelectedItem { get; set; }
        public IEnumerable<object> Items { get; set; } = [];
        public string PlaceHolder { get; set; } = "Select";
        public RrDropdown(string id)
        {
            Identifier = id;
        }
        public RrDropdown(string id, string tag)
        {
            Identifier = id;
            Tag = tag;
        }
    }

    public class RrPopup : IRrComponentBase
    {
        public string Identifier { get; set; }
        public bool Visible { get; set; }
        public string Text { get; set; }
        public string Tag { get; set; }
        public object StoredDataToDisplay { get; set; }
        public List<dynamic> Items { get; set; } = [];
        public RrPopup(string id)
        {
            Identifier = id;
        }
    }

    /// <summary>
    /// Used by IPeople.Roadrunner.Razor for custom razor component for RrScout
    /// </summary>
    public class RrPanel : IRrComponentBase
    {
        public string Identifier { get; set; }
        public bool Visible { get; set; }
        public string Size { get; set; }
        public string Tag { get; set; }
        public UIStates State { get; set; } = UIStates.Neutral;
        public PanelTypes? Type { get; set; }
        public bool Transition { get; set; }
        public RrPanel(string id)
        {
            Identifier = id;
        }
    }

    public class RrCheckBox
    {
        public string Identifier { get; set; }
        public string Text { get; set; }
        public bool IsChecked { get; set; }
    }

    public class RrPanelSetting
    {
        public string Name { get; set; }
        public RrPanelSettingTypes Type { get; set; }
        public bool Active { get; set; }
        public bool IsInverse { get; set; }
        public List<SettingColumnCondition> Conditions { get; set; } = [];
    }
    public class SettingColumnCondition
    {
        public ColumnConditionType Type { get; set; }
        public string ColumnName { get; set; }
        public string ColumnValue { get; set; }
        public SettingEffectType EffectType { get; set; }
        public bool Value { get; set; }
    }

    public enum SettingEffectType
    {
        Row,
        Column,
    }

    public enum ColumnConditionType
    {
        ValueEquals,
        ColumnExists,
    }

    public enum RrPanelSettingTypes
    {
        Slider,
        Button,
    }
    /// <summary>
    /// Used by IPeople.Roadrunner.Razor for custom razor component for RrScout
    /// </summary>
    public class RrPanelTab
    {
        public string Name { get; set; }
        public RenderFragment Content { get; set; }
        public Type ComponentType { get; set; }
        public UIStates SettingsUIState { get; set; } = UIStates.Neutral;
        public List<RrPanelSetting> Settings { get; set; }
        public object? Data { get; set; }
        public bool IsSelected { get; set; } = false;
        public bool Visible { get; set; } = true;
        public bool Disabled { get; set; } = false;
        public RrPanel Panel { get; set; }
    }

    public enum UIStates
    {
        Expanded,
        Collapsed,
        Neutral,
        Peaking
    }
}
