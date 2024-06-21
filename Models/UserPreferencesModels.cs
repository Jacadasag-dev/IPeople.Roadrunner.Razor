
namespace IPeople.Roadrunner.Razor.Models
{
    public class ColumnUserPreferences
    {
        public Dictionary<string, List<ColumnKVP<string, double>>> TableColumnOrder { get; set; } = new();
        public Dictionary<string, List<double>> TableColumnSizes { get; set; } = new Dictionary<string, List<double>>();
        public Dictionary<string, List<ColumnKVP<string, double>>> TableColumnWidth { get; set; } = new();
    }


    public class ColumnKVP<TKey, TValue>
    {
        public TKey Key { get; set; }
        public TValue Value { get; set; }

        public ColumnKVP(TKey key, TValue value)
        {
            Key = key;
            Value = value;
        }
    }

    public class PanelSettingsPreferences
    {
        public Dictionary<string, List<RrPanelSetting>> TabSettings { get; set; } = new();
    }
}
