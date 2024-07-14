using System.ComponentModel;
using System.Linq.Expressions;
using System.Reflection;
using IPeople.Roadrunner.Razor.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Microsoft.JSInterop.Infrastructure;

namespace IPeople.Roadrunner.Razor.Services
{
    public class RrStateService : IRrStateService
    {
        public class GlobalVariables
        {
            public RrLoading? Loading { get; set; }
            public PageBodyBounds? BodyBounds { get; set; }
        }

        public GlobalVariables AppGlobalVariables { get; set; } = new();
        public Dictionary<Type, Dictionary<string, IRrComponentBase>> Components { get; set; } = new();
        public event Action? RefreshAllComponents;
        public event Action<List<string>?>? RefreshSpecificComponentsById;
        public event Action<List<string>?>? RefreshSpecificComponentsByTag;

        #region Register/Remove Component
        public void RegisterComponent<T>(T component) where T : IRrComponentBase
        {
            if (string.IsNullOrEmpty(component.Id))
            {
                throw new ArgumentException("Component must have a non-empty Id", nameof(component));
            }

            Type componentType = typeof(T);
            if (!Components.ContainsKey(componentType))
            {
                Components[componentType] = new Dictionary<string, IRrComponentBase>();
            }
            Components[componentType][component.Id] = component;
        }
        public void RemoveComponent<T>(string id) where T : IRrComponentBase
        {
            Type componentType = typeof(T);
            if (Components.ContainsKey(componentType) && Components[componentType].ContainsKey(id))
            {
                Components[componentType].Remove(id);
            }
        }
        #endregion

        #region Refresh Component
        private HashSet<string> refreshingComponents = new HashSet<string>();
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

        #region Get Component
        public T? GetComponent<T>(T component) where T : class, IRrComponentBase
        {
            if (component == null || string.IsNullOrEmpty(component.Id))
            {
                throw new ArgumentException("Component must have a non-empty Id", nameof(component));
            }

            Type componentType = typeof(T);
            if (Components.ContainsKey(componentType) && Components[componentType].ContainsKey(component.Id))
            {
                return Components[componentType][component.Id] as T;
            }
            return null;
        }
        public T? GetComponentById<T>(string? id) where T : class, IRrComponentBase
        {
            if (string.IsNullOrEmpty(id))
                return default;

            Type componentType = typeof(T);
            if (Components.ContainsKey(componentType) && Components[componentType].ContainsKey(id))
            {
                return Components[componentType][id] as T;
            }
            return default;
        }
        public List<T>? GetComponentsByTag<T>(string? tag) where T : IRrComponentBase
        {
            if (string.IsNullOrEmpty(tag))
                return default;

            Type componentType = typeof(T);
            var componentsWithTag = new List<T>();
            if (Components.ContainsKey(componentType))
            {
                var componentDict = Components[componentType];
                foreach (var component in componentDict.Values)
                {
                    if (component is T typedComponent && typedComponent.Tag == tag)
                    {
                        componentsWithTag.Add(typedComponent);
                    }
                }
            }
            return componentsWithTag;
        }
        #endregion

        #region Set Component
        public void SetComponentPropertyById<T, TProperty>(string? id, Expression<Func<T, TProperty?>> propertySelector, TProperty? newValue, bool refresh = false) where T : class, IRrComponentBase
        {
            if (string.IsNullOrEmpty(id))
                return;

            Type componentType = typeof(T);
            if (!Components.ContainsKey(componentType))
                return;

            var componentDict = Components[componentType];

            if (!componentDict.ContainsKey(id))
                return;

            T? component = componentDict[id] as T;

            if (component is null || propertySelector is null)
                return;

            List<string> propertyPath = GetPropertyPath(propertySelector.Body);
            object targetObject = component;

            for (int i = 0; i < propertyPath.Count - 1; i++)
            {
                PropertyInfo propertyInfo = targetObject?.GetType().GetProperty(propertyPath[i]);
                targetObject = propertyInfo?.GetValue(targetObject);
            }

            PropertyInfo finalPropertyInfo = targetObject?.GetType().GetProperty(propertyPath.Last());

            if (finalPropertyInfo != null && finalPropertyInfo.CanWrite)
            {
                finalPropertyInfo.SetValue(targetObject, newValue);
            }

            // Prevent stack overflow by tracking refreshing components
            if (refresh && !refreshingComponents.Contains(id))
            {
                try
                {
                    refreshingComponents.Add(id);
                    RefreshComponentsById(id);
                }
                finally
                {
                    refreshingComponents.Remove(id);
                }
            }
        }
        public void SetComponentsPropertiesByTag<T, TProperty>(string? tag, Expression<Func<T, TProperty?>> propertySelector, TProperty? newValue, bool refresh = false) where T : class, IRrComponentBase
        {
            if (string.IsNullOrEmpty(tag))
                return;

            Type componentType = typeof(T);
            if (!Components.ContainsKey(componentType))
                return;

            var componentDict = Components[componentType];
            var componentIdsToRefresh = new List<string>();

            foreach (var kvp in componentDict)
            {
                T? component = kvp.Value as T;
                if (component != null && component.Tag == tag)
                {
                    List<string> propertyPath = GetPropertyPath(propertySelector.Body);
                    object targetObject = component;

                    for (int i = 0; i < propertyPath.Count - 1; i++)
                    {
                        PropertyInfo propertyInfo = targetObject?.GetType().GetProperty(propertyPath[i]);
                        targetObject = propertyInfo?.GetValue(targetObject);
                    }

                    PropertyInfo finalPropertyInfo = targetObject?.GetType().GetProperty(propertyPath.Last());

                    if (finalPropertyInfo != null && finalPropertyInfo.CanWrite)
                    {
                        finalPropertyInfo.SetValue(targetObject, newValue);
                        componentIdsToRefresh.Add(component.Id);
                    }
                }
            }

            // Optionally refresh the components
            if (refresh && componentIdsToRefresh.Any())
            {
                RefreshComponentsByTag(new List<string> { tag });
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

        #region Get/Set Component
        public TProperty? GetPropertyIfIsNotNullElseIfNullSetToNewValueAndReturnNewValue<T, TProperty>(T? rrComponent, Expression<Func<T, TProperty?>> propertySelector, TProperty? newValue) where T : class, IRrComponentBase
        {
            if (rrComponent is null)
                return default;

            T? component = GetComponent(rrComponent);

            if (component is null)
                RegisterComponent(rrComponent);

            component = GetComponent(rrComponent);

            if (component is null || propertySelector is null)
                return default;

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
                if (finalPropertyInfo.GetValue(targetObject) is null)
                {
                    finalPropertyInfo.SetValue(targetObject, newValue);
                    return newValue;
                }
                else if (finalPropertyInfo.GetValue(targetObject) is not null)
                {
                    return (TProperty?)finalPropertyInfo.GetValue(targetObject);
                }
            }
            
            return default;
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
        #endregion
    }
}
