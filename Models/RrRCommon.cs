
using IPeople.Roadrunner.Razor.Components;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace IPeople.Roadrunner.Razor.Models
{
    public class RrPanelDto
    {
        public string? Id { get; set; }
        public string? Size { get; set; }
        public string? PType { get; set; }
        public bool Latching { get; set; }
        public string? LatchingType { get; set; }
        public int MinLatchingWidth { get; set; }
        public string? State { get; set; }
        public DotNetObjectReference<RrPanel>? DotNetObjectReference { get; set; }
    }

    #region RrComponent Instance Models
    public interface IRrComponentBase
    {
        public string Id { get; set; }
        public bool Visible { get; set; }
        public string Tag { get; set; }
    }

    public class RrLoadingBase
    {
        public RrLoadingType Type { get; set; }
        public bool IsLoading { get; set; } = false;
        public string? Message { get; set; } = "Authenticating...";
    }
    #endregion
}
