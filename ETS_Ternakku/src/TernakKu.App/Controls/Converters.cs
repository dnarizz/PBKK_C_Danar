using System.Collections;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace TernakKu.App.Controls;

public class BoolToVisibleConverter : IValueConverter
{
    public bool Invert { get; set; }
    public object Convert(object value, Type t, object p, CultureInfo c) => (value is true) ^ Invert ? Visibility.Visible : Visibility.Collapsed;
    public object ConvertBack(object value, Type t, object p, CultureInfo c) => (value is Visibility.Visible) ^ Invert;
}

// Text shown in a ComboBox's closed box: the selected item's DisplayMemberPath property (dotted paths allowed).
public class DisplayMemberConverter : IMultiValueConverter
{
    public object? Convert(object[] values, Type t, object p, CultureInfo c)
    {
        var item = values.Length > 0 ? values[0] : null;
        var path = values.Length > 1 ? values[1] as string : null;
        if (item is null || item == DependencyProperty.UnsetValue) return "";
        if (string.IsNullOrEmpty(path)) return item.ToString();
        object? v = item;
        foreach (var part in path.Split('.'))
            v = v?.GetType().GetProperty(part)?.GetValue(v);
        return v?.ToString() ?? "";
    }
    public object[] ConvertBack(object value, Type[] t, object p, CultureInfo c) => throw new NotSupportedException();
}

// "on" when the value's text equals the parameter; used to mark the selected option in a button group.
public class EqualsConverter : IValueConverter
{
    public object? Convert(object value, Type t, object p, CultureInfo c) => string.Equals(value?.ToString(), p?.ToString(), StringComparison.Ordinal) ? "on" : null;
    public object ConvertBack(object value, Type t, object p, CultureInfo c) => throw new NotSupportedException();
}

public class NotConverter : IValueConverter
{
    public object Convert(object value, Type t, object p, CultureInfo c) => value is not true;
    public object ConvertBack(object value, Type t, object p, CultureInfo c) => value is not true;
}

// Collapses for null, empty strings, zero and empty lists.
public class EmptyToCollapsedConverter : IValueConverter
{
    public bool Invert { get; set; }
    public object Convert(object? value, Type t, object p, CultureInfo c)
    {
        var empty = value switch
        {
            null => true,
            string s => s.Length == 0,
            int i => i == 0,
            double d => d == 0,
            ICollection col => col.Count == 0,
            _ => false
        };
        return empty ^ Invert ? Visibility.Collapsed : Visibility.Visible;
    }
    public object ConvertBack(object value, Type t, object p, CultureInfo c) => throw new NotSupportedException();
}

// Farm scene index (0, 1, 2) to the matching "Scene.N" geometry.
public class SceneGeometryConverter : IValueConverter
{
    public object? Convert(object value, Type t, object p, CultureInfo c) =>
        Application.Current.TryFindResource($"Scene.{value}") as Geometry;
    public object ConvertBack(object value, Type t, object p, CultureInfo c) => throw new NotSupportedException();
}

// Number of stars to a GridLength; 0 hides the column.
public class StarConverter : IValueConverter
{
    public object Convert(object value, Type t, object p, CultureInfo c) =>
        value is double d && d > 0 ? new GridLength(d, GridUnitType.Star) : new GridLength(0);
    public object ConvertBack(object value, Type t, object p, CultureInfo c) => throw new NotSupportedException();
}

// Brush key name (e.g. "PrimaryBrush") to the brush from the current theme.
public class BrushKeyConverter : IValueConverter
{
    public object? Convert(object value, Type t, object p, CultureInfo c) =>
        value is string key ? Application.Current.TryFindResource(key) as Brush : null;
    public object ConvertBack(object value, Type t, object p, CultureInfo c) => throw new NotSupportedException();
}

// 0..100 to a height or width fraction of the parameter (default 1.0), for fills like silos and pens.
public class PercentToLengthConverter : IValueConverter
{
    public object Convert(object value, Type t, object p, CultureInfo c)
    {
        var full = p is string s && double.TryParse(s, NumberStyles.Any, CultureInfo.InvariantCulture, out var f) ? f : 100;
        return value is double d ? Math.Clamp(d, 0, 100) / 100 * full : 0.0;
    }
    public object ConvertBack(object value, Type t, object p, CultureInfo c) => throw new NotSupportedException();
}
