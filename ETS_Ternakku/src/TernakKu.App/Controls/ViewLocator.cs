using System.Windows;
using System.Windows.Controls;

namespace TernakKu.App.Controls;

// Finds the view for a dialog ViewModel by name: ViewModels.Dialogs.XxxViewModel -> Views.Dialogs.XxxView.
public class ViewLocator : DataTemplateSelector
{
    static readonly Dictionary<Type, DataTemplate> Cache = [];

    public override DataTemplate? SelectTemplate(object? item, DependencyObject container)
    {
        if (item is null) return null;
        var vmType = item.GetType();
        if (Cache.TryGetValue(vmType, out var cached)) return cached;
        var viewName = vmType.FullName!.Replace(".ViewModels.", ".Views.").Replace("ViewModel", "View");
        var viewType = vmType.Assembly.GetType(viewName)
            ?? throw new InvalidOperationException($"No view {viewName} for {vmType.Name}");
        var template = new DataTemplate(vmType) { VisualTree = new FrameworkElementFactory(viewType) };
        template.Seal();
        Cache[vmType] = template;
        return template;
    }
}
