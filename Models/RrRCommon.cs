
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

    public class RrLoading
    {
        public RrLoadingType Type { get; set; } = RrLoadingType.Authenticating;
        public bool IsLoading { get; set; } = true;
        public string Message { get; set; } = "Authenticating...";
    }
    #endregion
}
