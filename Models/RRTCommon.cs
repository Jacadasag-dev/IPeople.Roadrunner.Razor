
using Microsoft.AspNetCore.Components;

namespace IPeople.Roadrunner.Razor.Models
{
    public class PageBodyBounds
    {
        public int TopPosition { get; set; }
        public int LeftPosition { get; set; }
        public int RightPosition { get; set; }
        public int BottomPosition { get; set; }
        public int Height { get; set; }
        public int Width { get; set; }
    }

    #region RrComponent Instance Models
    public interface IRrComponentBase
    {
        public string Identifier { get; set; }
        public bool Visible { get; set; }
        public string Tag { get; set; }
    }

    public class RrCheckbox : IRrComponentBase
    {
        public string Identifier { get; set; }
        public bool Visible { get; set; }
        public string? Text { get; set; }
        public string? Tag { get; set; }
        public bool IsChecked { get; set; }
        public RrCheckbox(string id)
        {
            Identifier = id;
        }
    }

    public class RrInput : IRrComponentBase
    {
        public string Identifier { get; set; }
        public bool Visible { get; set; }
        public string? Text { get; set; }
        public string? Tag { get; set; }
        public string? PlaceHolder { get; set; }
        public bool DoDeBounce { get; set; } = true;
        public RrInput(string id)
        {
            Identifier = id;
        }
    }

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

    public class RrLoading
    {
        public RrLoadingType Type { get; set; } = RrLoadingType.Authenticating;
        public bool IsLoading { get; set; } = true;
        public string Message { get; set; } = "Authenticating...";
    }
    #endregion
}
