using IPeople.Roadrunner.Razor.Models;
using System.Linq.Expressions;

namespace IPeople.Roadrunner.Razor.Services
{
    public interface IRrStateService
    {
        RrStateService.GlobalVariables AppGlobalVariables { get; set; }
        RrStateService.ComponentInstances Components { get; set; }
        event Action<IRrComponentBase> OnUpdatePreference;
        event Action OnComponentChange;
        event Action OnClickOutOf;
        void RefreshComponents();
        void RegisterComponent(IRrComponentBase rrComponent);
        void RemoveComponent(IRrComponentBase rrComponent);
        void RegisterOrReplaceComponent(IRrComponentBase rrComponent);
        void UpdatePreference(IRrComponentBase rrComponent);
        void ClickOutOfException(object objectToMakeExceptionFor = null);
        void ClickOutOf();
        IRrComponentBase GetComponent<T>(IRrComponentBase rrComponent) where T : IRrComponentBase;
        IRrComponentBase GetComponentById<T>(string id) where T : IRrComponentBase;
        void SetComponentProperty<T, TProperty>(IRrComponentBase rrComponent, Expression<Func<T, TProperty>> propertySelector, TProperty newValue) where T : class, IRrComponentBase;
        void SetComponentPropertyById<T, TProperty>(string componentId, Expression<Func<T, TProperty>> propertySelector, TProperty newValue) where T : class, IRrComponentBase;
        void SetSelectedTab(RrPanelTab tab);
        void ToggleTabSettingsExpand(RrPanelTab tab);
        void SetTabSettingsExpandState(RrPanelTab tab, SettingUIStates state);
        void ToggleTabSetting(RrPanel panel, RrPanelTab tab, string settingName);
    }
}
