
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

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
        public string Id { get; set; }
        public bool Visible { get; set; }
        public string Tag { get; set; }
    }

    public class RrCheckbox : IRrComponentBase
    {
        public string Id { get; set; }
        public bool Visible { get; set; }
        public string? Text { get; set; }
        public string? Tag { get; set; }
        public bool IsChecked { get; set; }
        public RrCheckbox(string id)
        {
            Id = id;
        }
    }

    public class RrInput : IRrComponentBase
    {
        public string Id { get; set; }
        public bool Visible { get; set; }
        public string? Text { get; set; }
        public string? Tag { get; set; }
        public string? PlaceHolder { get; set; }
        public bool DoDeBounce { get; set; } = true;
        public RrInput(string id)
        {
            Id = id;
        }
    }

    public class RrDropdown : IRrComponentBase
    {
        public string Id { get; set; }
        public bool Visible { get; set; }
        public string? Tag { get; set; }
        public string? Label { get; set; }
        public object? SelectedItem { get; set; }
        public IEnumerable<object> Items { get; set; } = [];
        public string PlaceHolder { get; set; } = "Select";
        public RrDropdown(string id)
        {
            Id = id;
        }
        public RrDropdown(string id, string tag)
        {
            Id = id;
            Tag = tag;
        }
    }

    public class RrPopup : IRrComponentBase
    {
        public string Id { get; set; }
        public bool Visible { get; set; }
        public string Text { get; set; }
        public string Tag { get; set; }
        public object StoredDataToDisplay { get; set; }
        public List<dynamic> Items { get; set; } = [];
        public RrPopup(string id)
        {
            Id = id;
        }
    }

    public class RrPanel<T> : IRrComponentBase where T : class
    {
        public string Id { get; set; }
        public bool Visible { get; set; }
        public string? Size { get; set; }
        public string Tag { get; set; }
        public UIStates State { get; set; } = UIStates.Neutral;
        public PanelTypes? Type { get; set; }
        public bool Transition { get; set; }
        public DotNetObjectReference<T> DotNetReference { get; set; }

        public RrPanel(string id, T instance)
        {
            Id = id;
            DotNetReference = DotNetObjectReference.Create(instance);
        }

        public void Dispose()
        {
            DotNetReference?.Dispose();
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
