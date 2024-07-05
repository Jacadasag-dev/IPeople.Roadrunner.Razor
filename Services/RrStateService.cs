using System.ComponentModel;
using System.Linq.Expressions;
using System.Reflection;
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
        public event Action<IRrComponentBase>? OnUpdatePreference;
        public event Action? RefreshAllComponents;
        public event Action<List<string>>? RefreshSpecificComponentsById;
        public void RefreshComponents() { RefreshAllComponents?.Invoke(); }

        public void RefreshComponentsById(List<string> componentIds)
        {
            RefreshSpecificComponentsById?.Invoke(componentIds);
        }

        public void RefreshComponentsById(string componentId)
        {
            RefreshComponentsById(new List<string> { componentId });
        }

        public void RegisterComponent(IRrComponentBase rrComponent)
        {
            if (rrComponent == null)
                return;

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

        public void RegisterComponentById<T>(string id)
        {
            if (typeof(T) == typeof(RrInput))
            {
                var rrInput = new RrInput(id);
                RegisterOrAddComponent(Components.RrInputs, rrInput);
            }
            else if (typeof(T) == typeof(RrDropdown))
            {
                var rrDropdown = new RrDropdown(id);
                RegisterOrAddComponent(Components.RrDropdowns, rrDropdown);
            }
            else if (typeof(T) == typeof(RrPanel))
            {
                var rrPanel = new RrPanel(id);
                RegisterOrAddComponent(Components.RrPanels, rrPanel);
            }
            else
            {
                throw new Exception("Component or component type invalid...");
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

        public void SynchronizeComponent<T>(IRrComponentBase? rrComponent) where T : IRrComponentBase
        {
            if (rrComponent is null)
                return;

            if (typeof(T) == typeof(RrInput))
            {
                SyncComponent((RrInput)rrComponent, Components.RrInputs);
            }
            else if (typeof(T) == typeof(RrDropdown))
            {
                SyncComponent((RrDropdown)rrComponent, Components.RrDropdowns);
            }
            else if (typeof(T) == typeof(RrPanel))
            {
                SyncComponent((RrPanel)rrComponent, Components.RrPanels);
            }
            else
            {
                throw new Exception("Component or component type invalid...");
            }
        }

        /// Will cause infinite loop if the component calls itself through the RefreshComponentsById method
        public void SynchronizeComponentById<T>(string? componentId) where T : IRrComponentBase
        {
            if (string.IsNullOrEmpty(componentId))
                return;

            IRrComponentBase? rrComponent = GetComponentById<T>(componentId);
            if (rrComponent is null)
                return;

            if (typeof(T) == typeof(RrInput))
            {
                SyncComponent((RrInput)rrComponent, Components.RrInputs);
            }
            else if (typeof(T) == typeof(RrDropdown))
            {
                SyncComponent((RrDropdown)rrComponent, Components.RrDropdowns);
            }
            else if (typeof(T) == typeof(RrPanel))
            {
                SyncComponent((RrPanel)rrComponent, Components.RrPanels);
            }
            else
            {
                throw new Exception("Component or component type invalid...");
            }
            RefreshComponentsById(rrComponent.Identifier);
        }

        private void SyncComponent<TComponent>(TComponent newComponent, List<TComponent> componentList) where TComponent : IRrComponentBase
        {
            var existingComponent = componentList.FirstOrDefault(c => c.Identifier == newComponent.Identifier);
            if (existingComponent != null)
            {
                var index = componentList.IndexOf(existingComponent);
                componentList[index] = newComponent;
            }
        }


        public void UpdatePreference(IRrComponentBase rrComponent)
        {
            OnUpdatePreference?.Invoke(rrComponent);
        }
       
        public IRrComponentBase? GetComponent<T>(IRrComponentBase rrComponent) where T : IRrComponentBase
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
        public IRrComponentBase? GetComponentById<T>(string id) where T : IRrComponentBase
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
        public void SetComponentProperty<T, TProperty>(IRrComponentBase? rrComponent, Expression<Func<T, TProperty?>> propertySelector, TProperty? newValue) where T : class, IRrComponentBase
        {
            if (rrComponent is null)
                return;

            var component = GetComponent<T>(rrComponent) as T;
            if (component is not null && propertySelector is not null)
            {
                // Get the property path from the expression
                var propertyPath = GetPropertyPath(propertySelector.Body);

                // Get the actual property to set the value
                object? targetObject = component as object;
                for (int i = 0; i < propertyPath.Count - 1; i++)
                {
                    PropertyInfo? propertyInfo = targetObject?.GetType().GetProperty(propertyPath[i]);
                    targetObject = propertyInfo?.GetValue(targetObject);
                }

                var finalPropertyInfo = targetObject?.GetType().GetProperty(propertyPath.Last());
                if (finalPropertyInfo != null && finalPropertyInfo.CanWrite)
                {
                    finalPropertyInfo.SetValue(targetObject, newValue);
                }
            }
        }
        public void SetComponentPropertyById<T, TProperty>(string componentId, Expression<Func<T, TProperty?>> propertySelector, TProperty? newValue) where T : class, IRrComponentBase
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
                    var propertyInfo = targetObject?.GetType().GetProperty(propertyPath[i]);
                    targetObject = propertyInfo?.GetValue(targetObject);
                }

                var finalPropertyInfo = targetObject?.GetType().GetProperty(propertyPath.Last());
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
                if (tabInstance.SettingsUIState == UIStates.Collapsed)
                {
                    tabInstance.SettingsUIState = UIStates.Expanded;
                }
                else if (tabInstance.SettingsUIState == UIStates.Neutral)
                {
                    tabInstance.SettingsUIState = UIStates.Expanded;
                }
                else
                {
                    tabInstance.SettingsUIState = UIStates.Collapsed;
                }
            }
        }
        public void SetTabSettingsExpandState(RrPanelTab tab, UIStates state)
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

        public string? GetDisplayValue(object? item)
        {
            if (item == null)
            {
                return "null";
            }

            switch (item)
            {
                case string strItem:
                    return strItem;
                case Enum enumItem:
                    return enumItem.ToString();
            }

            var type = item.GetType();

            // Check for property or field named "Name"
            var nameMember = type.GetMember("Name").FirstOrDefault();
            if (nameMember != null && (nameMember.MemberType == MemberTypes.Property || nameMember.MemberType == MemberTypes.Field))
            {
                var value = GetMemberValue(item, nameMember);
                if (value is string nameValue)
                {
                    return nameValue;
                }
            }

            // Check for property or field named "Text"
            var textMember = type.GetMember("Text").FirstOrDefault();
            if (textMember != null && (textMember.MemberType == MemberTypes.Property || textMember.MemberType == MemberTypes.Field))
            {
                var value = GetMemberValue(item, textMember);
                if (value is string textValue)
                {
                    return textValue;
                }
            }

            // Check for any string properties
            var stringProperty = type.GetProperties().FirstOrDefault(p => p.PropertyType == typeof(string));
            if (stringProperty != null)
            {
                return stringProperty.GetValue(item) as string;
            }

            // Check for any string fields
            var stringField = type.GetFields().FirstOrDefault(f => f.FieldType == typeof(string));
            if (stringField != null)
            {
                return stringField.GetValue(item) as string;
            }

            return type.Name;
        }

        private object? GetMemberValue(object obj, MemberInfo member)
        {
            return member switch
            {
                PropertyInfo property => property.GetValue(obj),
                FieldInfo field => field.GetValue(obj),
                _ => null
            };
        }

    }
}
