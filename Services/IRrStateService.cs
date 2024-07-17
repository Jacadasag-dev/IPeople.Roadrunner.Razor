using IPeople.Roadrunner.Razor.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System.Linq.Expressions;

namespace IPeople.Roadrunner.Razor.Services
{
    public interface IRrStateService
    {
        Dictionary<Type, Dictionary<string, IRrComponentBase>> Components { get; set; }
        event Action? RefreshAllComponents;
        event Action<List<string>?>? RefreshSpecificComponentsById;
        event Action<List<string>?>? RefreshSpecificComponentsByTag;
        event Action<string, RrLoadingBase>? LoadingStateChangeRequestById;
        event Action<string, RrLoadingBase>? LoadingStateChangeRequestByTag;
        event Action? StopAllLoading;
        void RegisterComponent<T>(T component) where T : IRrComponentBase;
        void RemoveComponent<T>(string id) where T : IRrComponentBase;
        void RefreshComponents();
        void SetComponentLoadStatusById(string id, RrLoadingBase loading);
        void SetComponentLoadStatusByTag(string tag, RrLoadingBase loading);
        void StopAllComponentLoading();
        void RefreshComponentsById(List<string> componentIds);
        void RefreshComponentsById(string componentId);
        void RefreshComponentsByTag(List<string> componentTags);
        void RefreshComponentsByTag(string componentTag);
        T? GetComponent<T>(T component) where T : class, IRrComponentBase;
        T? GetComponentById<T>(string? id) where T : class, IRrComponentBase;
        List<T>? GetComponentsByTag<T>(string? tag) where T : IRrComponentBase;
        void SetComponentPropertyById<T, TProperty>(string? id, Expression<Func<T, TProperty?>> propertySelector, TProperty? newValue, bool refresh = false) where T : class, IRrComponentBase;
        void SetComponentsPropertiesByTag<T, TProperty>(string? tag, Expression<Func<T, TProperty?>> propertySelector, TProperty? newValue, bool refresh = false) where T : class, IRrComponentBase;
        TProperty? GetPropertyIfIsNotNullElseIfNullSetToNewValueAndReturnNewValue<T, TProperty>(T? rrComponent, Expression<Func<T, TProperty?>> propertySelector, TProperty? newValue) where T : class, IRrComponentBase;
        string? GetDisplayValue(object? item);
    }
}
