using System.Windows;

namespace TernakKu.App.Services;

// Swaps the colour dictionary (Themes/Light.xaml or Themes/Dark.xaml) at runtime.
public static class ThemeService
{
    public static event Action? Changed;

    public static void Apply(string theme)
    {
        var dicts = Application.Current.Resources.MergedDictionaries;
        var uri = new Uri($"pack://application:,,,/TernakKu;component/Themes/{(theme == "dark" ? "Dark" : "Light")}.xaml", UriKind.Absolute);
        var old = dicts.FirstOrDefault(d => d.Source is { } s && (s.OriginalString.EndsWith("Light.xaml") || s.OriginalString.EndsWith("Dark.xaml")));
        var fresh = new ResourceDictionary { Source = uri };
        if (old is null) dicts.Insert(0, fresh);
        else dicts[dicts.IndexOf(old)] = fresh;
        Changed?.Invoke();
    }
}
