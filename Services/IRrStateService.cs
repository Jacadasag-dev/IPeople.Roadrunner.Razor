using IPeople.Roadrunner.Razor.Models;
using System.Linq.Expressions;

namespace IPeople.Roadrunner.Razor.Services
{
    public interface IRrStateService
    {
        RrStateService.GlobalVariables AppGlobalVariables { get; set; }
        RrStateService.ComponentInstances Components { get; set; }
        event Action? RefreshAllComponents;
        event Action<List<string>?>? RefreshSpecificComponentsById;
        event Action<List<string>?>? RefreshSpecificComponentsByTag;
        void RefreshComponents();
        void RefreshComponentsById(List<string> componentIds);
        void RefreshComponentsById(string componentId);
        void RefreshComponentsByTag(List<string> componentTags);
        void RefreshComponentsByTag(string componentTag);
        void RegisterComponent(IRrComponentBase rrComponent);
        void RegisterComponentById<T>(string id);
        void RemoveComponent(IRrComponentBase rrComponent);
        void RegisterOrReplaceComponent(IRrComponentBase rrComponent);
        T? GetComponent<T>(IRrComponentBase rrComponent) where T : class, IRrComponentBase;
        T? GetComponentById<T>(string id) where T : class, IRrComponentBase;
        List<T> GetComponentsByTag<T>(string tag) where T : class, IRrComponentBase;
        void SetComponentProperty<T, TProperty>(IRrComponentBase? rrComponent, Expression<Func<T, TProperty?>> propertySelector, TProperty? newValue) where T : class, IRrComponentBase;
        void SetComponentPropertyById<T, TProperty>(string componentId, Expression<Func<T, TProperty?>> propertySelector, TProperty? newValue) where T : class, IRrComponentBase;
        void SetComponentsPropertyByTag<T, TProperty>(string componentTag, Expression<Func<T, TProperty?>> propertySelector, TProperty? newValue) where T : class, IRrComponentBase;
        string? GetDisplayValue(object? item);
    }
}
