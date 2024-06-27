using System.Linq.Expressions;
using IPeople.Roadrunner.Razor.Models;

namespace IPeople.Roadrunner.Razor.Services
{
    public class RrStateService : IRrStateService
    {
        public class GlobalVariables
        {
            public RrLoading Loading { get; set; }
        }

        public class ComponentInstances
        {
            public List<RrInput> RrInputs { get; set; } = [];
            public List<RrDropdown> RrDropdowns { get; set; } = [];
            public List<RrPanel> RrPanels { get; set; } = [];
            public List<RrPopup> RrPopups { get; set; } = [];
        }

        public GlobalVariables AppGlobalVariables { get; set; } = new();
        public ComponentInstances Components { get; set; } = new();
        public event Action<IRrComponentBase> OnUpdatePreference;
        public event Action OnComponentChange;
        public event Action OnClickOutOf;
        private CancellationTokenSource _cts = new CancellationTokenSource();
        private readonly object _lock = new object();
        public void RefreshComponents() { OnComponentChange?.Invoke(); }
        public void RegisterComponent(IRrComponentBase rrComponent)
        {
            switch (rrComponent)
            {
                case RrInput rrInput:
                    RegisterOrAddComponent(Components.RrInputs, rrInput);
                    break;
                case RrDropdown rrDropdown:
                    RegisterOrAddComponent(Components.RrDropdowns, rrDropdown);
                    break;
                case RrPanel rrPanel:
                    RegisterOrAddComponent(Components.RrPanels, rrPanel);
                    break;
                case RrPopup rrPopup:
                    RegisterOrAddComponent(Components.RrPopups, rrPopup);
                    break;
                default:
                    throw new ArgumentException("Unsupported component type", nameof(rrComponent));
            }
        }
        public void RemoveComponent(IRrComponentBase rrComponent)
        {
            switch (rrComponent)
            {
                case RrInput rrInput:
                    RemoveOrIgnoreComponent(Components.RrInputs, rrInput);
                    break;
                case RrDropdown rrDropdown:
                    RemoveOrIgnoreComponent(Components.RrDropdowns, rrDropdown);
                    break;
                case RrPanel rrPanel:
                    RemoveOrIgnoreComponent(Components.RrPanels, rrPanel);
                    break;
                case RrPopup rrPopup:
                    RemoveOrIgnoreComponent(Components.RrPopups, rrPopup);
                    break;
                default:
                    throw new ArgumentException("Unsupported component type", nameof(rrComponent));
            }
        }
        public void RegisterOrReplaceComponent(IRrComponentBase rrComponent)
        {
            switch (rrComponent)
            {
                case RrInput rrInput:
                    RegisterOrReplaceComponent(Components.RrInputs, rrInput);
                    break;
                case RrDropdown rrDropdown:
                    RegisterOrReplaceComponent(Components.RrDropdowns, rrDropdown);
                    break;
                case RrPanel rrPanel:
                    RegisterOrReplaceComponent(Components.RrPanels, rrPanel);
                    break;
                default:
                    throw new ArgumentException("Unsupported component type", nameof(rrComponent));
            }
        }
        private void RegisterOrAddComponent<T>(List<T> componentList, T component) where T : IRrComponentBase
        {
            if (!componentList.Any(c => c.Identifier == component.Identifier))
            {
                componentList.Add(component);
            }
        }
        private void RegisterOrReplaceComponent<T>(List<T> componentList, T component) where T : IRrComponentBase
        {
            var existingComponent = componentList.FirstOrDefault(c => c.Identifier == component.Identifier);
            if (existingComponent != null)
            {
                var index = componentList.IndexOf(existingComponent);
                componentList[index] = component;
            }
            else
            {
                componentList.Add(component);
            }
        }
        private void RemoveOrIgnoreComponent<T>(List<T> componentList, T component) where T : IRrComponentBase
        {
            var existingComponent = componentList.FirstOrDefault(c => c.Identifier == component.Identifier);
            if (existingComponent != null)
            {
                componentList.Remove(existingComponent);
            }
        }

        public void UpdatePreference(IRrComponentBase rrComponent)
        {
            OnUpdatePreference?.Invoke(rrComponent);
        }
        public void ClickOutOfException(object objectToMakeExceptionFor = null)
        {
            lock (_lock)
            {
                _cts.Cancel(); // Cancel the ongoing delay
            }
            if (objectToMakeExceptionFor is RrDropdown clickedDropdown)
            {
                if (clickedDropdown != null)
                {
                    // Toggle the UI state of the clicked dropdown
                    var dropdownInstance = GetComponentById<RrDropdown>(clickedDropdown.Identifier) as RrDropdown;
                    if (dropdownInstance != null)
                    {
                        dropdownInstance.UIState = dropdownInstance.UIState == SettingUIStates.Expanded ? SettingUIStates.Collapsed : SettingUIStates.Expanded;
                    }
                    // Set the UI state of other dropdowns
                    foreach (var dropdown in Components.RrDropdowns.Where(d => d.Identifier != clickedDropdown.Identifier))
                    {
                        dropdown.UIState = dropdown.UIState == SettingUIStates.Expanded ? SettingUIStates.Collapsed : SettingUIStates.Neutral;
                    }
                }
            }
            else
            {
                foreach (var expandedDropdown in Components.RrDropdowns.Where(d => d.UIState == SettingUIStates.Expanded))
                {
                    expandedDropdown.UIState = SettingUIStates.Collapsed;
                }
                OnClickOutOf?.Invoke();
            }
        }
        public async void ClickOutOf()
        {
            CancellationToken token;
            lock (_lock)
            {
                token = _cts.Token;
                _cts = new CancellationTokenSource(); // Reset for future use
            }
            try
            {
                await Task.Delay(50, token); // Wait for split second or until cancellation
                Components.RrDropdowns.ForEach(d => d.UIState = d.UIState == SettingUIStates.Expanded ? SettingUIStates.Collapsed : SettingUIStates.Neutral);
                OnClickOutOf?.Invoke();
            }
            catch (TaskCanceledException)
            {
                // Handle cancellation if necessary
            }
        }
        public IRrComponentBase GetComponent<T>(IRrComponentBase rrComponent) where T : IRrComponentBase
        {
            if (typeof(T) == typeof(RrInput))
            {
                return Components.RrInputs.FirstOrDefault(i => i.Identifier == rrComponent.Identifier);
            }
            else if (typeof(T) == typeof(RrDropdown))
            {
                return Components.RrDropdowns.FirstOrDefault(d => d.Identifier == rrComponent.Identifier);
            }
            else if (typeof(T) == typeof(RrPanel))
            {
                return Components.RrPanels.FirstOrDefault(p => p.Identifier == rrComponent.Identifier);
            }
            else
            {
                throw new Exception("Component or component type invalid...");
            }
        }
        public IRrComponentBase GetComponentById<T>(string id) where T : IRrComponentBase
        {
            if (typeof(T) == typeof(RrInput))
            {
                return Components.RrInputs.FirstOrDefault(i => i.Identifier == id);
            }
            else if (typeof(T) == typeof(RrDropdown))
            {
                return Components.RrDropdowns.FirstOrDefault(d => d.Identifier == id);
            }
            else if (typeof(T) == typeof(RrPanel))
            {
                return Components.RrPanels.FirstOrDefault(p => p.Identifier == id);
            }
            else
            {
                throw new Exception("Component or component type invalid...");
            }
        }
        public void SetComponentProperty<T, TProperty>(IRrComponentBase rrComponent, Expression<Func<T, TProperty>> propertySelector, TProperty newValue) where T : class, IRrComponentBase
        {
            var component = GetComponent<T>(rrComponent) as T;
            if (component != null)
            {
                // Get the property path from the expression
                var propertyPath = GetPropertyPath(propertySelector.Body);

                // Get the actual property to set the value
                var targetObject = component as object;
                for (int i = 0; i < propertyPath.Count - 1; i++)
                {
                    var propertyInfo = targetObject.GetType().GetProperty(propertyPath[i]);
                    targetObject = propertyInfo.GetValue(targetObject);
                }

                var finalPropertyInfo = targetObject.GetType().GetProperty(propertyPath.Last());
                if (finalPropertyInfo != null && finalPropertyInfo.CanWrite)
                {
                    finalPropertyInfo.SetValue(targetObject, newValue);
                }
            }
        }
        public void SetComponentPropertyById<T, TProperty>(string componentId, Expression<Func<T, TProperty>> propertySelector, TProperty newValue) where T : class, IRrComponentBase
        {
            var component = GetComponentById<T>(componentId) as T;
            if (component != null)
            {
                // Get the property path from the expression
                var propertyPath = GetPropertyPath(propertySelector.Body);

                // Get the actual property to set the value
                var targetObject = component as object;
                for (int i = 0; i < propertyPath.Count - 1; i++)
                {
                    var propertyInfo = targetObject.GetType().GetProperty(propertyPath[i]);
                    targetObject = propertyInfo.GetValue(targetObject);
                }

                var finalPropertyInfo = targetObject.GetType().GetProperty(propertyPath.Last());
                if (finalPropertyInfo != null && finalPropertyInfo.CanWrite)
                {
                    finalPropertyInfo.SetValue(targetObject, newValue);
                }
            }
        }
        private List<string> GetPropertyPath(Expression expression)
        {
            var path = new List<string>();

            while (expression is MemberExpression memberExpression)
            {
                path.Insert(0, memberExpression.Member.Name);
                expression = memberExpression.Expression;
            }

            if (expression is ParameterExpression)
            {
                return path;
            }

            throw new ArgumentException("The property selector expression is not valid.", nameof(expression));
        }
        public void SetSelectedTab(RrPanelTab tab)
        {
            var panelInstance = Components.RrPanels.FirstOrDefault(p => p.Identifier == tab.Panel.Identifier && p.Tabs.Any(t => t.Name == tab.Name));
            if (panelInstance != null)
            {
                panelInstance.Tabs.ForEach(t => t.IsSelected = t.Name == tab.Name);
            }
        }
        public void ToggleTabSettingsExpand(RrPanelTab tab)
        {
            var tabInstance = (GetComponentById<RrPanel>(tab.Panel.Identifier) as RrPanel)?.Tabs.FirstOrDefault(t => t.Name == tab.Name);
            if (tabInstance != null)
            {
                if (tabInstance.SettingsUIState == SettingUIStates.Collapsed)
                {
                    tabInstance.SettingsUIState = SettingUIStates.Expanded;
                }
                else if (tabInstance.SettingsUIState == SettingUIStates.Neutral)
                {
                    tabInstance.SettingsUIState = SettingUIStates.Expanded;
                }
                else
                {
                    tabInstance.SettingsUIState = SettingUIStates.Collapsed;
                }
            }
        }
        public void SetTabSettingsExpandState(RrPanelTab tab, SettingUIStates state)
        {
            var tabInstance = (GetComponentById<RrPanel>(tab.Panel.Identifier) as RrPanel)?.Tabs.FirstOrDefault(t => t.Name == tab.Name);
            if (tabInstance != null)
            {
                tabInstance.SettingsUIState = state;
            }
        }
        public void ToggleTabSetting(RrPanel panel, RrPanelTab tab, string settingName)
        {
            var panelInstance = Components.RrPanels.FirstOrDefault(p => p.Identifier == panel.Identifier);
            if (panelInstance != null)
            {
                if (panelInstance.Tabs is not null && panelInstance.Tabs.Any() && tab != null)
                {
                    var tabInstance = panelInstance.Tabs.FirstOrDefault(t => t.Name == tab.Name) ?? panelInstance.Tabs.First();
                    if (tabInstance != null)
                    {
                        var matchedSetting = tabInstance.Settings.FirstOrDefault(s => s.Name == settingName);
                        if (matchedSetting != null)
                        {
                            matchedSetting.Active = !matchedSetting.Active;
                        }
                    }
                }
            }
        }
    }
}
