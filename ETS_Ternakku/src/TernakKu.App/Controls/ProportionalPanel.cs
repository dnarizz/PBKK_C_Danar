using System.Windows;
using System.Windows.Controls;

namespace TernakKu.App.Controls;

// Lays children in one row with widths proportional to their Weight (like CSS flex-grow), separated by Gap.
public class ProportionalPanel : Panel
{
    public static readonly DependencyProperty WeightProperty = DependencyProperty.RegisterAttached(
        "Weight", typeof(double), typeof(ProportionalPanel), new FrameworkPropertyMetadata(1.0, FrameworkPropertyMetadataOptions.AffectsParentArrange));
    public static double GetWeight(DependencyObject o) => (double)o.GetValue(WeightProperty);
    public static void SetWeight(DependencyObject o, double v) => o.SetValue(WeightProperty, v);

    public static readonly DependencyProperty GapProperty = DependencyProperty.Register(
        nameof(Gap), typeof(double), typeof(ProportionalPanel), new FrameworkPropertyMetadata(8.0, FrameworkPropertyMetadataOptions.AffectsArrange));
    public double Gap { get => (double)GetValue(GapProperty); set => SetValue(GapProperty, value); }

    IEnumerable<UIElement> Visible => InternalChildren.Cast<UIElement>().Where(c => c.Visibility != Visibility.Collapsed);

    // In an ItemsControl, set Weight on the item container through ItemContainerStyle.
    static double WeightOf(UIElement c) => GetWeight(c);

    protected override Size MeasureOverride(Size available)
    {
        double h = 0;
        foreach (var c in Visible) { c.Measure(new Size(double.PositiveInfinity, available.Height)); h = Math.Max(h, c.DesiredSize.Height); }
        return new Size(double.IsInfinity(available.Width) ? 0 : available.Width, h);
    }

    protected override Size ArrangeOverride(Size final)
    {
        var kids = Visible.ToList();
        if (kids.Count == 0) return final;
        var total = kids.Sum(WeightOf);
        var free = final.Width - Gap * (kids.Count - 1);
        double x = 0;
        foreach (var c in kids)
        {
            var w = free * WeightOf(c) / (total == 0 ? 1 : total);
            c.Arrange(new Rect(x, 0, Math.Max(w, 0), final.Height));
            x += w + Gap;
        }
        return final;
    }
}
