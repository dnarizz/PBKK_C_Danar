using System.Windows;
using System.Windows.Controls;

namespace TernakKu.App.Controls;

// Small status pill. Tone: ok, warn, danger, info, neutral. The look lives in Themes/Controls.xaml.
public class Badge : Control
{
    static Badge() => DefaultStyleKeyProperty.OverrideMetadata(typeof(Badge), new FrameworkPropertyMetadata(typeof(Badge)));

    public static readonly DependencyProperty TextProperty = DependencyProperty.Register(nameof(Text), typeof(string), typeof(Badge), new PropertyMetadata(""));
    public static readonly DependencyProperty ToneProperty = DependencyProperty.Register(nameof(Tone), typeof(string), typeof(Badge), new PropertyMetadata("neutral"));
    public static readonly DependencyProperty ShowDotProperty = DependencyProperty.Register(nameof(ShowDot), typeof(bool), typeof(Badge), new PropertyMetadata(true));

    public string Text { get => (string)GetValue(TextProperty); set => SetValue(TextProperty, value); }
    public string Tone { get => (string)GetValue(ToneProperty); set => SetValue(ToneProperty, value); }
    public bool ShowDot { get => (bool)GetValue(ShowDotProperty); set => SetValue(ShowDotProperty, value); }
}
