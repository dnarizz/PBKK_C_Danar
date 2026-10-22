using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;

namespace TernakKu.App.Controls;

// Draws a stroke icon from Themes/Icons.xaml ("Icon.<Kind>") on a 24x24 grid, coloured by the inherited Foreground.
public class Icon : FrameworkElement
{
    static Icon()
    {
        VerticalAlignmentProperty.OverrideMetadata(typeof(Icon), new FrameworkPropertyMetadata(VerticalAlignment.Center));
        HorizontalAlignmentProperty.OverrideMetadata(typeof(Icon), new FrameworkPropertyMetadata(HorizontalAlignment.Center));
    }

    public static readonly DependencyProperty KindProperty = DependencyProperty.Register(
        nameof(Kind), typeof(string), typeof(Icon), new FrameworkPropertyMetadata("", FrameworkPropertyMetadataOptions.AffectsRender));

    public static readonly DependencyProperty SizeProperty = DependencyProperty.Register(
        nameof(Size), typeof(double), typeof(Icon), new FrameworkPropertyMetadata(18.0, FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsRender));

    public static readonly DependencyProperty ThicknessProperty = DependencyProperty.Register(
        nameof(Thickness), typeof(double), typeof(Icon), new FrameworkPropertyMetadata(1.75, FrameworkPropertyMetadataOptions.AffectsRender));

    public static readonly DependencyProperty ForegroundProperty = TextElement.ForegroundProperty.AddOwner(
        typeof(Icon), new FrameworkPropertyMetadata(Brushes.Black, FrameworkPropertyMetadataOptions.Inherits | FrameworkPropertyMetadataOptions.AffectsRender));

    public string Kind { get => (string)GetValue(KindProperty); set => SetValue(KindProperty, value); }
    public double Size { get => (double)GetValue(SizeProperty); set => SetValue(SizeProperty, value); }
    public double Thickness { get => (double)GetValue(ThicknessProperty); set => SetValue(ThicknessProperty, value); }
    public Brush Foreground { get => (Brush)GetValue(ForegroundProperty); set => SetValue(ForegroundProperty, value); }

    protected override System.Windows.Size MeasureOverride(System.Windows.Size availableSize) => new(Size, Size);

    protected override void OnRender(DrawingContext dc)
    {
        if (string.IsNullOrEmpty(Kind) || TryFindResource($"Icon.{Kind}") is not Geometry g) return;
        var s = Size / 24.0;
        var pen = new Pen(Foreground, Thickness) { StartLineCap = PenLineCap.Round, EndLineCap = PenLineCap.Round, LineJoin = PenLineJoin.Round };
        dc.PushTransform(new ScaleTransform(s, s));
        dc.DrawGeometry(null, pen, g);
        dc.Pop();
    }
}

// Draws a line illustration (any Geometry) scaled uniformly from a fixed design grid into the element's width.
public class LineArt : FrameworkElement
{
    public static readonly DependencyProperty DataProperty = DependencyProperty.Register(
        nameof(Data), typeof(Geometry), typeof(LineArt), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender));

    public static readonly DependencyProperty GridWidthProperty = DependencyProperty.Register(
        nameof(GridWidth), typeof(double), typeof(LineArt), new FrameworkPropertyMetadata(260.0, FrameworkPropertyMetadataOptions.AffectsMeasure));

    public static readonly DependencyProperty GridHeightProperty = DependencyProperty.Register(
        nameof(GridHeight), typeof(double), typeof(LineArt), new FrameworkPropertyMetadata(110.0, FrameworkPropertyMetadataOptions.AffectsMeasure));

    public static readonly DependencyProperty StrokeProperty = DependencyProperty.Register(
        nameof(Stroke), typeof(Brush), typeof(LineArt), new FrameworkPropertyMetadata(Brushes.Black, FrameworkPropertyMetadataOptions.AffectsRender));

    public static readonly DependencyProperty ThicknessProperty = DependencyProperty.Register(
        nameof(Thickness), typeof(double), typeof(LineArt), new FrameworkPropertyMetadata(2.0, FrameworkPropertyMetadataOptions.AffectsRender));

    public Geometry? Data { get => (Geometry?)GetValue(DataProperty); set => SetValue(DataProperty, value); }
    public double GridWidth { get => (double)GetValue(GridWidthProperty); set => SetValue(GridWidthProperty, value); }
    public double GridHeight { get => (double)GetValue(GridHeightProperty); set => SetValue(GridHeightProperty, value); }
    public Brush Stroke { get => (Brush)GetValue(StrokeProperty); set => SetValue(StrokeProperty, value); }
    public double Thickness { get => (double)GetValue(ThicknessProperty); set => SetValue(ThicknessProperty, value); }

    protected override Size MeasureOverride(Size available)
    {
        var w = double.IsInfinity(available.Width) ? (double.IsNaN(Width) ? GridWidth : Width) : available.Width;
        return new Size(w, w * GridHeight / GridWidth);
    }

    protected override void OnRender(DrawingContext dc)
    {
        if (Data is null || ActualWidth <= 0) return;
        var s = ActualWidth / GridWidth;
        var pen = new Pen(Stroke, Thickness) { StartLineCap = PenLineCap.Round, EndLineCap = PenLineCap.Round, LineJoin = PenLineJoin.Round };
        dc.PushTransform(new ScaleTransform(s, s));
        dc.DrawGeometry(null, pen, Data);
        dc.Pop();
    }
}
