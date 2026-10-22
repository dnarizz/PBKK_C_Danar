using System.Windows;
using System.Windows.Controls;

namespace TernakKu.App.Controls;

// Attached properties read by the templates in Themes/Controls.xaml.
public static class Ui
{
    public static readonly DependencyProperty PlaceholderProperty = DependencyProperty.RegisterAttached(
        "Placeholder", typeof(string), typeof(Ui), new PropertyMetadata(""));
    public static string GetPlaceholder(DependencyObject o) => (string)o.GetValue(PlaceholderProperty);
    public static void SetPlaceholder(DependencyObject o, string v) => o.SetValue(PlaceholderProperty, v);

    // Icon shown at the start of an input (key from Themes/Icons.xaml without the "Icon." prefix).
    public static readonly DependencyProperty IconProperty = DependencyProperty.RegisterAttached(
        "Icon", typeof(string), typeof(Ui), new PropertyMetadata(""));
    public static string GetIcon(DependencyObject o) => (string)o.GetValue(IconProperty);
    public static void SetIcon(DependencyObject o, string v) => o.SetValue(IconProperty, v);

    // Red border for a field that failed validation.
    public static readonly DependencyProperty InvalidProperty = DependencyProperty.RegisterAttached(
        "Invalid", typeof(bool), typeof(Ui), new PropertyMetadata(false));
    public static bool GetInvalid(DependencyObject o) => (bool)o.GetValue(InvalidProperty);
    public static void SetInvalid(DependencyObject o, bool v) => o.SetValue(InvalidProperty, v);

    // Trailing unit text inside an input, e.g. "kg".
    public static readonly DependencyProperty SuffixProperty = DependencyProperty.RegisterAttached(
        "Suffix", typeof(string), typeof(Ui), new PropertyMetadata(""));
    public static string GetSuffix(DependencyObject o) => (string)o.GetValue(SuffixProperty);
    public static void SetSuffix(DependencyObject o, string v) => o.SetValue(SuffixProperty, v);

    // PasswordBox has no bindable text; this flag lets the template show the placeholder while it is empty.
    public static readonly DependencyProperty IsEmptyProperty = DependencyProperty.RegisterAttached(
        "IsEmpty", typeof(bool), typeof(Ui), new PropertyMetadata(true));
    public static bool GetIsEmpty(DependencyObject o) => (bool)o.GetValue(IsEmptyProperty);
    public static void SetIsEmpty(DependencyObject o, bool v) => o.SetValue(IsEmptyProperty, v);

    public static readonly DependencyProperty TrackEmptyProperty = DependencyProperty.RegisterAttached(
        "TrackEmpty", typeof(bool), typeof(Ui), new PropertyMetadata(false, (o, e) =>
        {
            if (o is not PasswordBox pb || e.NewValue is not true) return;
            pb.PasswordChanged += (_, _) => SetIsEmpty(pb, pb.Password.Length == 0);
        }));
    public static bool GetTrackEmpty(DependencyObject o) => (bool)o.GetValue(TrackEmptyProperty);
    public static void SetTrackEmpty(DependencyObject o, bool v) => o.SetValue(TrackEmptyProperty, v);
}
