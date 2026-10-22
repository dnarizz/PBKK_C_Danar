using System.Globalization;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;
using TernakKu.App.Services;

namespace TernakKu.App.Controls;

// Small hand-drawn charts (no chart library). They redraw when data changes or the theme switches.
public abstract class ChartBase : FrameworkElement
{
    protected static readonly CultureInfo Id = CultureInfo.GetCultureInfo("id-ID");

    public static readonly DependencyProperty FontFamilyProperty = TextElement.FontFamilyProperty.AddOwner(typeof(ChartBase));

    protected ChartBase()
    {
        Loaded += (_, _) => ThemeService.Changed += Redraw;
        Unloaded += (_, _) => ThemeService.Changed -= Redraw;
    }

    void Redraw() => InvalidateVisual();

    protected Brush B(string key) => TryFindResource(key) as Brush ?? Brushes.Gray;

    protected FormattedText Text(string s, double size, Brush brush, bool bold = false) =>
        new(s, Id, FlowDirection.LeftToRight,
            new Typeface((FontFamily)GetValue(FontFamilyProperty), FontStyles.Normal, bold ? FontWeights.Bold : FontWeights.Normal, FontStretches.Normal),
            size, brush, VisualTreeHelper.GetDpi(this).PixelsPerDip);

    protected static double NiceMax(double v)
    {
        if (v <= 0) return 1;
        var m = Math.Pow(10, Math.Floor(Math.Log10(v)));
        foreach (var f in new[] { 1, 1.5, 2, 2.5, 3, 4, 5, 6, 8, 10 })
            if (f * m >= v) return f * m;
        return 10 * m;
    }

    protected static Geometry TopRounded(Rect r, double radius)
    {
        radius = Math.Min(radius, Math.Min(r.Width / 2, r.Height));
        var g = new StreamGeometry();
        using var c = g.Open();
        c.BeginFigure(new Point(r.Left, r.Bottom), true, true);
        c.LineTo(new Point(r.Left, r.Top + radius), false, false);
        c.ArcTo(new Point(r.Left + radius, r.Top), new Size(radius, radius), 0, false, SweepDirection.Clockwise, false, false);
        c.LineTo(new Point(r.Right - radius, r.Top), false, false);
        c.ArcTo(new Point(r.Right, r.Top + radius), new Size(radius, radius), 0, false, SweepDirection.Clockwise, false, false);
        c.LineTo(new Point(r.Right, r.Bottom), false, false);
        g.Freeze();
        return g;
    }

    protected static FrameworkPropertyMetadata Render(object? def = null) => new(def, FrameworkPropertyMetadataOptions.AffectsRender);
}

public class BarChart : ChartBase
{
    public static readonly DependencyProperty ValuesProperty = DependencyProperty.Register(nameof(Values), typeof(IList<double>), typeof(BarChart), Render());
    public static readonly DependencyProperty LabelsProperty = DependencyProperty.Register(nameof(Labels), typeof(IList<string>), typeof(BarChart), Render());
    public IList<double>? Values { get => (IList<double>?)GetValue(ValuesProperty); set => SetValue(ValuesProperty, value); }
    public IList<string>? Labels { get => (IList<string>?)GetValue(LabelsProperty); set => SetValue(LabelsProperty, value); }

    protected override void OnRender(DrawingContext dc)
    {
        var v = Values;
        if (v is null || v.Count == 0 || ActualWidth <= 0) return;
        double padL = 34, padT = 22, padB = 22, gap = 8;
        double w = ActualWidth, h = ActualHeight, pw = w - padL, ph = h - padT - padB;
        var max = NiceMax(v.Max());
        var grid = new Pen(B("DividerBrush"), 1);
        var muted = B("Text2Brush");
        foreach (var f in new[] { 0.0, 0.5, 1.0 })
        {
            var y = padT + ph - ph * f;
            dc.DrawLine(f == 0 ? new Pen(B("BorderStrongBrush"), 1) : grid, new Point(padL, y), new Point(w, y));
            var t = Text((max * f).ToString("N0", Id), 10.5, muted);
            dc.DrawText(t, new Point(padL - 6 - t.Width, y - t.Height / 2));
        }
        var bw = (pw - gap * (v.Count - 1)) / v.Count;
        for (var i = 0; i < v.Count; i++)
        {
            var bh = Math.Max(2, ph * v[i] / max);
            var x = padL + i * (bw + gap);
            var last = i == v.Count - 1;
            dc.DrawGeometry(last ? B("PrimaryBrush") : B("LeafBrush"), null, TopRounded(new Rect(x, padT + ph - bh, bw, bh), 4));
            if (last)
            {
                var lt = Text(v[i].ToString("N0", Id), 11.5, B("TextBrush"), true);
                dc.DrawText(lt, new Point(x + bw / 2 - lt.Width / 2, padT + ph - bh - lt.Height - 2));
            }
            if (Labels is { } l && i < l.Count)
            {
                var t = Text(l[i], 10.5, last ? B("TextBrush") : muted, last);
                dc.DrawText(t, new Point(x + bw / 2 - t.Width / 2, h - t.Height));
            }
        }
    }
}

public class LineChart : ChartBase
{
    public static readonly DependencyProperty ValuesProperty = DependencyProperty.Register(nameof(Values), typeof(IList<double>), typeof(LineChart), Render());
    public static readonly DependencyProperty LabelsProperty = DependencyProperty.Register(nameof(Labels), typeof(IList<string>), typeof(LineChart), Render());
    public static readonly DependencyProperty TargetProperty = DependencyProperty.Register(nameof(Target), typeof(double?), typeof(LineChart), Render());
    public static readonly DependencyProperty UnitProperty = DependencyProperty.Register(nameof(Unit), typeof(string), typeof(LineChart), Render("kg"));
    public IList<double>? Values { get => (IList<double>?)GetValue(ValuesProperty); set => SetValue(ValuesProperty, value); }
    public IList<string>? Labels { get => (IList<string>?)GetValue(LabelsProperty); set => SetValue(LabelsProperty, value); }
    public double? Target { get => (double?)GetValue(TargetProperty); set => SetValue(TargetProperty, value); }
    public string Unit { get => (string)GetValue(UnitProperty); set => SetValue(UnitProperty, value); }

    protected override void OnRender(DrawingContext dc)
    {
        var v = Values;
        if (v is null || v.Count == 0 || ActualWidth <= 0) return;
        var all = Target is double tg ? v.Append(tg).ToList() : v.ToList();
        double lo = all.Min(), hi = all.Max(), pad = (hi - lo) * 0.15;
        if (pad == 0) pad = 5;
        lo = Math.Max(0, Math.Floor((lo - pad) / 5) * 5); hi = Math.Ceiling((hi + pad) / 5) * 5;
        double padL = 40, padT = 14, padB = 22, padR = 10, w = ActualWidth, h = ActualHeight;
        double pw = w - padL - padR, ph = h - padT - padB;
        double X(int i) => padL + (v.Count == 1 ? pw / 2 : pw * i / (v.Count - 1));
        double Y(double val) => padT + ph - ph * (val - lo) / (hi - lo == 0 ? 1 : hi - lo);
        var muted = B("Text2Brush");
        foreach (var f in new[] { 0.0, 0.5, 1.0 })
        {
            var val = lo + (hi - lo) * f;
            dc.DrawLine(new Pen(f == 0 ? B("BorderStrongBrush") : B("DividerBrush"), 1), new Point(padL, Y(val)), new Point(w - padR, Y(val)));
            var t = Text(val.ToString("N0", Id), 10.5, muted);
            dc.DrawText(t, new Point(padL - 6 - t.Width, Y(val) - t.Height / 2));
        }
        if (Target is double target)
        {
            var pen = new Pen(B("WarnBarBrush"), 1.5) { DashStyle = new DashStyle([4, 4], 0) };
            dc.DrawLine(pen, new Point(padL, Y(target)), new Point(w - padR, Y(target)));
            var t = Text($"Target {target:N0} {Unit}", 10.5, muted);
            dc.DrawText(t, new Point(w - padR - t.Width, Y(target) - t.Height - 2));
        }
        var line = new StreamGeometry();
        var area = new StreamGeometry();
        using (var c = line.Open())
        {
            c.BeginFigure(new Point(X(0), Y(v[0])), false, false);
            for (var i = 1; i < v.Count; i++) c.LineTo(new Point(X(i), Y(v[i])), true, true);
        }
        using (var c = area.Open())
        {
            c.BeginFigure(new Point(X(0), padT + ph), true, true);
            for (var i = 0; i < v.Count; i++) c.LineTo(new Point(X(i), Y(v[i])), false, false);
            c.LineTo(new Point(X(v.Count - 1), padT + ph), false, false);
        }
        var fill = B("LeafBrush").Clone(); fill.Opacity = 0.22;
        dc.DrawGeometry(fill, null, area);
        dc.DrawGeometry(null, new Pen(B("PrimaryBrush"), 2.5) { LineJoin = PenLineJoin.Round }, line);
        var every = (int)Math.Ceiling(v.Count / 8.0);
        for (var i = 0; i < v.Count; i++)
        {
            dc.DrawEllipse(B("SurfaceBrush"), new Pen(B("PrimaryBrush"), 2), new Point(X(i), Y(v[i])), 4, 4);
            if (Labels is { } l && i < l.Count && (i % every == 0 || i == v.Count - 1))
            {
                var t = Text(l[i], 10.5, muted);
                dc.DrawText(t, new Point(X(i) - t.Width / 2, h - t.Height));
            }
        }
        var lt = Text($"{v[^1]:N1} {Unit}", 11.5, B("TextBrush"), true);
        dc.DrawText(lt, new Point(X(v.Count - 1) - lt.Width - 6, Y(v[^1]) - lt.Height - 6));
    }
}

public class DonutChart : ChartBase
{
    public static readonly DependencyProperty ValuesProperty = DependencyProperty.Register(nameof(Values), typeof(IList<double>), typeof(DonutChart), Render());
    public static readonly DependencyProperty BrushKeysProperty = DependencyProperty.Register(nameof(BrushKeys), typeof(string), typeof(DonutChart), Render("PrimaryBrush,WarnBarBrush,InfoBrush"));
    public static readonly DependencyProperty RingProperty = DependencyProperty.Register(nameof(Ring), typeof(double), typeof(DonutChart), Render(16.0));
    public IList<double>? Values { get => (IList<double>?)GetValue(ValuesProperty); set => SetValue(ValuesProperty, value); }
    public string BrushKeys { get => (string)GetValue(BrushKeysProperty); set => SetValue(BrushKeysProperty, value); }
    public double Ring { get => (double)GetValue(RingProperty); set => SetValue(RingProperty, value); }

    protected override void OnRender(DrawingContext dc)
    {
        var size = Math.Min(ActualWidth, ActualHeight);
        if (size <= 0) return;
        var c = new Point(ActualWidth / 2, ActualHeight / 2);
        var r = (size - Ring) / 2;
        dc.DrawEllipse(null, new Pen(B("DividerBrush"), Ring), c, r, r);
        var v = Values;
        if (v is null || v.Sum() <= 0) return;
        var keys = BrushKeys.Split(',');
        var total = v.Sum();
        var start = -90.0;
        for (var i = 0; i < v.Count; i++)
        {
            var sweep = 360 * v[i] / total;
            if (sweep <= 0) continue;
            var gapDeg = v.Count(x => x > 0) > 1 ? 1.6 : 0;
            dc.DrawGeometry(null, new Pen(B(keys[i % keys.Length]), Ring), Arc(c, r, start + gapDeg / 2, Math.Max(sweep - gapDeg, 0.5)));
            start += sweep;
        }
    }

    static Geometry Arc(Point c, double r, double startDeg, double sweepDeg)
    {
        Point P(double deg) => new(c.X + r * Math.Cos(deg * Math.PI / 180), c.Y + r * Math.Sin(deg * Math.PI / 180));
        if (sweepDeg >= 359.9) return new EllipseGeometry(c, r, r);
        var g = new StreamGeometry();
        using var ctx = g.Open();
        ctx.BeginFigure(P(startDeg), false, false);
        ctx.ArcTo(P(startDeg + sweepDeg), new Size(r, r), 0, sweepDeg > 180, SweepDirection.Clockwise, true, false);
        return g;
    }
}

public class Sparkline : ChartBase
{
    public static readonly DependencyProperty ValuesProperty = DependencyProperty.Register(nameof(Values), typeof(IList<double>), typeof(Sparkline), Render());
    public IList<double>? Values { get => (IList<double>?)GetValue(ValuesProperty); set => SetValue(ValuesProperty, value); }

    protected override void OnRender(DrawingContext dc)
    {
        var v = Values;
        if (v is null || v.Count < 2 || ActualWidth <= 0) return;
        double lo = v.Min(), hi = v.Max(), w = ActualWidth, h = ActualHeight;
        double X(int i) => (w - 8) * i / (v.Count - 1) + 4;
        double Y(double val) => h - 6 - (h - 12) * (val - lo) / (hi - lo == 0 ? 1 : hi - lo);
        var line = new StreamGeometry();
        using (var c = line.Open())
        {
            c.BeginFigure(new Point(X(0), Y(v[0])), false, false);
            for (var i = 1; i < v.Count; i++) c.LineTo(new Point(X(i), Y(v[i])), true, true);
        }
        var area = new StreamGeometry();
        using (var c = area.Open())
        {
            c.BeginFigure(new Point(X(0), h), true, true);
            for (var i = 0; i < v.Count; i++) c.LineTo(new Point(X(i), Y(v[i])), false, false);
            c.LineTo(new Point(X(v.Count - 1), h), false, false);
        }
        var fill = B("LeafBrush").Clone(); fill.Opacity = 0.2;
        dc.DrawGeometry(fill, null, area);
        dc.DrawGeometry(null, new Pen(B("PrimaryBrush"), 2) { LineJoin = PenLineJoin.Round }, line);
        dc.DrawEllipse(B("SurfaceBrush"), new Pen(B("PrimaryBrush"), 2), new Point(X(v.Count - 1), Y(v[^1])), 3.5, 3.5);
    }
}

// Two bars per group (income vs spending).
public class GroupedBarChart : ChartBase
{
    public static readonly DependencyProperty MasukProperty = DependencyProperty.Register(nameof(Masuk), typeof(IList<double>), typeof(GroupedBarChart), Render());
    public static readonly DependencyProperty KeluarProperty = DependencyProperty.Register(nameof(Keluar), typeof(IList<double>), typeof(GroupedBarChart), Render());
    public static readonly DependencyProperty LabelsProperty = DependencyProperty.Register(nameof(Labels), typeof(IList<string>), typeof(GroupedBarChart), Render());
    public IList<double>? Masuk { get => (IList<double>?)GetValue(MasukProperty); set => SetValue(MasukProperty, value); }
    public IList<double>? Keluar { get => (IList<double>?)GetValue(KeluarProperty); set => SetValue(KeluarProperty, value); }
    public IList<string>? Labels { get => (IList<string>?)GetValue(LabelsProperty); set => SetValue(LabelsProperty, value); }

    protected override void OnRender(DrawingContext dc)
    {
        if (Masuk is not { Count: > 0 } a || Keluar is not { } b || ActualWidth <= 0) return;
        double padL = 52, padT = 14, padB = 22, gap = 20, w = ActualWidth, h = ActualHeight;
        double pw = w - padL, ph = h - padT - padB;
        var max = NiceMax(Math.Max(a.Max(), b.Max()));
        var muted = B("Text2Brush");
        foreach (var f in new[] { 0.0, 0.5, 1.0 })
        {
            var y = padT + ph - ph * f;
            dc.DrawLine(new Pen(f == 0 ? B("BorderStrongBrush") : B("DividerBrush"), 1), new Point(padL, y), new Point(w, y));
            var t = Text(Format.RupiahPendek(max * f).Replace("Rp ", ""), 10.5, muted);
            dc.DrawText(t, new Point(padL - 6 - t.Width, y - t.Height / 2));
        }
        var gw = (pw - gap * (a.Count - 1)) / a.Count;
        var bw = (gw - 6) / 2;
        for (var i = 0; i < a.Count; i++)
        {
            var x = padL + i * (gw + gap);
            var hi = Math.Max(1, ph * a[i] / max);
            var ho = Math.Max(1, ph * b[i] / max);
            dc.DrawGeometry(B("PrimaryBrush"), null, TopRounded(new Rect(x, padT + ph - hi, bw, hi), 3));
            dc.DrawGeometry(B("WarnBarBrush"), null, TopRounded(new Rect(x + bw + 6, padT + ph - ho, bw, ho), 3));
            if (Labels is { } l && i < l.Count)
            {
                var t = Text(l[i], 10.5, muted);
                dc.DrawText(t, new Point(x + gw / 2 - t.Width / 2, h - t.Height));
            }
        }
    }
}
