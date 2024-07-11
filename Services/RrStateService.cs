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
            public RrLoading? Loading { get; set; }
            public PageBodyBounds? BodyBounds { get; set; }
        }
        public class ComponentInstances
        {
            public List<RrInput> RrInputs { get; set; } = [];
            public List<RrDropdown> RrDropdowns { get; set; } = [];
            public List<RrPanel> RrPanels { get; set; } = [];
            public List<RrPopup> RrPopups { get; set; } = [];
            public List<RrCheckbox> RrCheckboxes { get; set; } = [];
        }
        public GlobalVariables AppGlobalVariables { get; set; } = new();
        public ComponentInstances Components { get; set; } = new();
        public event Action? RefreshAllComponents;
        public event Action<List<string>?>? RefreshSpecificComponentsById;
        public event Action<List<string>?>? RefreshSpecificComponentsByTag;

        #region Refresh Component
        public void RefreshComponents() { RefreshAllComponents?.Invoke(); }
        public void RefreshComponentsById(List<string> componentIds)
        {
            RefreshSpecificComponentsById?.Invoke(componentIds);
        }
        public void RefreshComponentsById(string componentId)
        {
            RefreshComponentsById(new List<string> { componentId });
        }
        public void RefreshComponentsByTag(List<string> componentTags)
        {
            RefreshSpecificComponentsByTag?.Invoke(componentTags);
        }
        public void RefreshComponentsByTag(string componentTag)
        {
            RefreshSpecificComponentsByTag?.Invoke(new List<string> { componentTag });
        }
        #endregion

        #region Register Component
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
                case RrCheckbox rrCheckbox:
                    RegisterOrAddComponent(Components.RrCheckboxes, rrCheckbox);
                    break;
                default:
                    throw new ArgumentException("Unsupported component type", nameof(rrComponent));
            }
        }
        public void RegisterComponentById<T>(string id)
        {
            switch (typeof(T))
            {
                case Type t when t == typeof(RrInput):
                    RegisterOrAddComponent(Components.RrInputs, new RrInput(id));
                    break;
                case Type t when t == typeof(RrDropdown):
                    RegisterOrAddComponent(Components.RrDropdowns, new RrDropdown(id));
                    break;
                case Type t when t == typeof(RrPanel):
                    RegisterOrAddComponent(Components.RrPanels, new RrPanel(id));
                    break;
                case Type t when t == typeof(RrCheckbox):
                    RegisterOrAddComponent(Components.RrCheckboxes, new RrCheckbox(id));
                    break;
                default:
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
                case RrCheckbox rrCheckbox:
                    RemoveOrIgnoreComponent(Components.RrCheckboxes, rrCheckbox);
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
                case RrCheckbox rrCheckbox:
                    RegisterOrReplaceComponent(Components.RrCheckboxes, rrCheckbox);
                    break;
                default:
                    throw new ArgumentException("Unsupported component type", nameof(rrComponent));
            }
        }
        #endregion

        #region Get Component
        public T? GetComponent<T>(IRrComponentBase rrComponent) where T : class, IRrComponentBase
        {
            switch (typeof(T))
            {
                case Type t when t == typeof(RrInput):
                    return Components.RrInputs.FirstOrDefault(i => i.Identifier == rrComponent.Identifier) as T;
                case Type t when t == typeof(RrDropdown):
                    return Components.RrDropdowns.FirstOrDefault(d => d.Identifier == rrComponent.Identifier) as T;
                case Type t when t == typeof(RrPanel):
                    return Components.RrPanels.FirstOrDefault(p => p.Identifier == rrComponent.Identifier) as T;
                case Type t when t == typeof(RrCheckbox):
                    return Components.RrCheckboxes.FirstOrDefault(p => p.Identifier == rrComponent.Identifier) as T;
                default:
                    throw new Exception("Component or component type invalid...");
            }
        }
        public T? GetComponentById<T>(string id) where T : class, IRrComponentBase
        {
            switch (typeof(T))
            {
                case Type t when t == typeof(RrInput):
                    return Components.RrInputs.FirstOrDefault(i => i.Identifier == id) as T;
                case Type t when t == typeof(RrDropdown):
                    return Components.RrDropdowns.FirstOrDefault(d => d.Identifier == id) as T;
                case Type t when t == typeof(RrPanel):
                    return Components.RrPanels.FirstOrDefault(p => p.Identifier == id) as T;
                case Type t when t == typeof(RrCheckbox):
                    return Components.RrCheckboxes.FirstOrDefault(p => p.Identifier == id) as T;
                default:
                    throw new Exception("Component or component type invalid...");
            }
        }
        public List<T> GetComponentsByTag<T>(string tag) where T : class, IRrComponentBase
        {
            List<T> components = new List<T>();
            switch (typeof(T))
            {
                case Type t when t == typeof(RrInput):
                    components.AddRange(Components.RrInputs
                        .Where(i => !string.IsNullOrEmpty(i.Tag) && i.Tag.Equals(tag, StringComparison.OrdinalIgnoreCase))
                        .Cast<T>()
                        .ToList());
                    break;
                case Type t when t == typeof(RrDropdown):
                    components.AddRange(Components.RrDropdowns
                        .Where(d => !string.IsNullOrEmpty(d.Tag) && d.Tag.Equals(tag, StringComparison.OrdinalIgnoreCase))
                        .Cast<T>()
                        .ToList());
                    break;
                case Type t when t == typeof(RrPanel):
                    components.AddRange(Components.RrPanels
                        .Where(p => !string.IsNullOrEmpty(p.Tag) && p.Tag.Equals(tag, StringComparison.OrdinalIgnoreCase))
                        .Cast<T>()
                        .ToList());
                    break;
                case Type t when t == typeof(RrCheckbox):
                    components.AddRange(Components.RrCheckboxes
                        .Where(p => !string.IsNullOrEmpty(p.Tag) && p.Tag.Equals(tag, StringComparison.OrdinalIgnoreCase))
                        .Cast<T>()
                        .ToList());
                    break;
                default:
                    throw new Exception($"Component type {typeof(T).Name} is invalid.");
            }
            return components;
        }
        #endregion

        #region Set Component
        public void SetComponentProperty<T, TProperty>(IRrComponentBase? rrComponent, Expression<Func<T, TProperty?>> propertySelector, TProperty? newValue) where T : class, IRrComponentBase
        {
            if (rrComponent is null)
                return;

            var component = GetComponent<T>(rrComponent) as T;
            if (component is not null && propertySelector is not null)
            {
                List<string>? propertyPath = GetPropertyPath(propertySelector.Body);
                object? targetObject = component as object;
                for (int i = 0; i < propertyPath.Count - 1; i++)
                {
                    PropertyInfo? propertyInfo = targetObject?.GetType().GetProperty(propertyPath[i]);
                    targetObject = propertyInfo?.GetValue(targetObject);
                }
                PropertyInfo? finalPropertyInfo = targetObject?.GetType().GetProperty(propertyPath.Last());
                if (finalPropertyInfo != null && finalPropertyInfo.CanWrite)
                {
                    finalPropertyInfo.SetValue(targetObject, newValue);
                }
            }
        }
        public void SetComponentPropertyById<T, TProperty>(string componentId, Expression<Func<T, TProperty?>> propertySelector, TProperty? newValue) where T : class, IRrComponentBase
        {
            var component = GetComponentById<T>(componentId);
            if (component != null)
            {
                List<string>? propertyPath = GetPropertyPath(propertySelector.Body);
                object? targetObject = component;
                for (int i = 0; i < propertyPath.Count - 1; i++)
                {
                    PropertyInfo? propertyInfo = targetObject?.GetType().GetProperty(propertyPath[i]);
                    targetObject = propertyInfo?.GetValue(targetObject);
                }
                PropertyInfo? finalPropertyInfo = targetObject?.GetType().GetProperty(propertyPath.Last());
                if (finalPropertyInfo != null && finalPropertyInfo.CanWrite)
                {
                    finalPropertyInfo.SetValue(targetObject, newValue);
                }
            }
        }
        public void SetComponentsPropertyByTag<T, TProperty>(string componentTag, Expression<Func<T, TProperty?>> propertySelector, TProperty? newValue) where T : class, IRrComponentBase
        {
            List<T> components = GetComponentsByTag<T>(componentTag)?.Cast<T>().ToList() ?? new List<T>();
            if (components == null || components.Count == 0)
                return;

            foreach (T component in components)
            {
                if (component == null)
                    continue;

                List<string>? propertyPath = GetPropertyPath(propertySelector.Body);
                object? targetObject = component;
                for (int i = 0; i < propertyPath.Count - 1; i++)
                {
                    PropertyInfo? propertyInfo = targetObject?.GetType().GetProperty(propertyPath[i]);
                    targetObject = propertyInfo?.GetValue(targetObject);
                }
                PropertyInfo? finalPropertyInfo = targetObject?.GetType().GetProperty(propertyPath.Last());
                if (finalPropertyInfo != null && finalPropertyInfo.CanWrite)
                    finalPropertyInfo.SetValue(targetObject, newValue);
            }
        }
        #endregion

        #region Helpers Methods
        public string? GetDisplayValue(object? item)
        {
            if (item is null)
                return "null";

            switch (item)
            {
                case string strItem:
                    return strItem;
                case Enum enumItem:
                    return enumItem.ToString();
            }
            Type? type = item.GetType();
            // Check for property or field named "Name"
            MemberInfo? nameMember = type.GetMember("Name").FirstOrDefault();
            if (nameMember != null && (nameMember.MemberType == MemberTypes.Property || nameMember.MemberType == MemberTypes.Field))
            {
                object? value = GetMemberValue(item, nameMember);
                if (value is string nameValue)
                {
                    return nameValue;
                }
            }
            // Check for property or field named "Text"
            MemberInfo? textMember = type.GetMember("Text").FirstOrDefault();
            if (textMember != null && (textMember.MemberType == MemberTypes.Property || textMember.MemberType == MemberTypes.Field))
            {
                object? value = GetMemberValue(item, textMember);
                if (value is string textValue)
                {
                    return textValue;
                }
            }
            // Check for any string properties
            PropertyInfo? stringProperty = type.GetProperties().FirstOrDefault(p => p.PropertyType == typeof(string));
            if (stringProperty != null)
            {
                return stringProperty.GetValue(item) as string;
            }
            // Check for any string fields
            FieldInfo? stringField = type.GetFields().FirstOrDefault(f => f.FieldType == typeof(string));
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
        private List<string> GetPropertyPath(Expression? expression)
        {
            List<string>? path = [];
            while (expression is MemberExpression memberExpression)
            {
                path.Insert(0, memberExpression.Member.Name);
                expression = memberExpression.Expression;
            }
            if (expression is ParameterExpression)
                return path;

            throw new ArgumentException("The property selector expression is not valid.", nameof(expression));
        }
        #endregion
    }
}
