using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Threading;
using TernakKu.App.ViewModels;

namespace TernakKu.App.Views;

public partial class ShellView : UserControl
{
    static readonly IEasingFunction Ease = new ExponentialEase { EasingMode = EasingMode.EaseOut, Exponent = 6 };
    static Duration Normal => SystemParameters.ClientAreaAnimation ? TimeSpan.FromMilliseconds(250) : TimeSpan.Zero;
    bool indPlaced;

    public ShellView()
    {
        InitializeComponent();
        DataContextChanged += (_, e) =>
        {
            if (e.OldValue is INotifyPropertyChanged o) o.PropertyChanged -= OnVmChanged;
            if (e.NewValue is INotifyPropertyChanged n) n.PropertyChanged += OnVmChanged;
        };
        Loaded += (_, _) => { ApplyCollapse(animate: false); QueueIndicator(); };
        navList.SizeChanged += (_, _) => { QueueIndicator(); FitArt(); };
        sidebar.SizeChanged += (_, _) => FitArt();
        PreviewKeyDown += OnKey;
    }

    ShellViewModel Vm => (ShellViewModel)DataContext;

    void OnVmChanged(object? sender, PropertyChangedEventArgs e)
    {
        switch (e.PropertyName)
        {
            case nameof(ShellViewModel.Halaman):
                pageScroll.ScrollToTop();
                Animate(page, OpacityProperty, 0, 1);
                pageShift.BeginAnimation(TranslateTransform.YProperty, new DoubleAnimation(12, 0, Normal) { EasingFunction = Ease });
                QueueIndicator();
                break;
            case nameof(ShellViewModel.Ciut):
                ApplyCollapse(animate: true);
                QueueIndicator();
                break;
            case nameof(ShellViewModel.Dialog) when Vm.Dialog is not null:
                Animate(dialogLayer, OpacityProperty, 0, 1);
                dialogScale.BeginAnimation(ScaleTransform.ScaleXProperty, new DoubleAnimation(0.96, 1, Normal) { EasingFunction = Ease });
                dialogScale.BeginAnimation(ScaleTransform.ScaleYProperty, new DoubleAnimation(0.96, 1, Normal) { EasingFunction = Ease });
                Dispatcher.BeginInvoke(DispatcherPriority.Loaded, FocusFirstField);
                break;
            case nameof(ShellViewModel.Snack) when Vm.Snack is not null:
                Animate(snack, OpacityProperty, 0, 1, 167);
                snackShift.BeginAnimation(TranslateTransform.YProperty, new DoubleAnimation(8, 0, TimeSpan.FromMilliseconds(167)) { EasingFunction = Ease });
                break;
        }
    }

    static void Animate(UIElement el, DependencyProperty p, double from, double to, int ms = 250) =>
        el.BeginAnimation(p, new DoubleAnimation(from, to, SystemParameters.ClientAreaAnimation ? TimeSpan.FromMilliseconds(ms) : TimeSpan.Zero) { EasingFunction = Ease });

    void ApplyCollapse(bool animate)
    {
        var ciut = Vm.Ciut;
        var target = ciut ? 72.0 : 252.0;
        if (animate) sidebar.BeginAnimation(WidthProperty, new DoubleAnimation(target, Normal) { EasingFunction = Ease });
        else { sidebar.BeginAnimation(WidthProperty, null); sidebar.Width = target; }
        var hide = ciut ? Visibility.Collapsed : Visibility.Visible;
        farmMeta.Visibility = hide;
        sidebarArt.Visibility = hide;
        who.Visibility = hide;
        farmCard.Margin = ciut ? new Thickness(10, 12, 10, 6) : new Thickness(12, 12, 12, 6);
        farmCard.Padding = ciut ? new Thickness(6, 10, 6, 10) : new Thickness(10);
        farmCard.Background = ciut ? Brushes.Transparent : (Brush)FindResource("SidebarCardBrush");
        foot.Background = ciut ? Brushes.Transparent : (Brush)FindResource("SidebarFootBrush");
        navArea.Margin = ciut ? new Thickness(10, 0, 10, 8) : new Thickness(12, 0, 12, 8);
        ((Grid)foot.Child).ColumnDefinitions[2].Width = GridLength.Auto;
        logout.SetValue(Grid.ColumnProperty, ciut ? 0 : 2);
        logout.SetValue(Grid.RowProperty, 0);
        logout.Margin = ciut ? new Thickness(0, 44, 0, 0) : new Thickness(0);
        foot.Height = ciut ? 88 : double.NaN;
    }

    // The small farm drawing gives way when the menu needs the height, so no menu item is cut off.
    void FitArt()
    {
        if (Vm.Ciut) return;
        var artHeight = sidebarArt.Visibility == Visibility.Visible ? sidebarArt.ActualHeight + sidebarArt.Margin.Top : 0;
        var room = navScroll.ActualHeight + artHeight;
        var need = navArea.ActualHeight + navArea.Margin.Bottom;
        sidebarArt.Visibility = need + 80 <= room ? Visibility.Visible : Visibility.Collapsed;
    }

    void QueueIndicator() => Dispatcher.BeginInvoke(DispatcherPriority.Loaded, PlaceIndicator);

    // Slide the cream pill to the active nav item.
    void PlaceIndicator()
    {
        var active = Vm.Nav.FirstOrDefault(n => n.IsActive);
        if (active is null || navList.ItemContainerGenerator.ContainerFromItem(active) is not ContentPresenter cp) { navInd.Opacity = 0; return; }
        var btn = FindChild<Button>(cp, "navBtn");
        if (btn is null || !btn.IsLoaded) return;
        var y = btn.TranslatePoint(new Point(0, 0), navArea).Y;
        navInd.Height = btn.ActualHeight;
        navInd.Opacity = 1;
        if (!indPlaced) { navIndShift.BeginAnimation(TranslateTransform.YProperty, null); navIndShift.Y = y; indPlaced = true; return; }
        navIndShift.BeginAnimation(TranslateTransform.YProperty, new DoubleAnimation(y, Normal) { EasingFunction = Ease });
    }

    static T? FindChild<T>(DependencyObject parent, string name) where T : FrameworkElement
    {
        for (var i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
        {
            var child = VisualTreeHelper.GetChild(parent, i);
            if (child is T t && t.Name == name) return t;
            if (FindChild<T>(child, name) is { } found) return found;
        }
        return null;
    }

    void FocusFirstField()
    {
        var first = FindFirstFocusable(dialogBody);
        first?.Focus();
    }

    static Control? FindFirstFocusable(DependencyObject parent)
    {
        for (var i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
        {
            var child = VisualTreeHelper.GetChild(parent, i);
            if (child is TextBox or ComboBox or DatePicker && ((Control)child).IsEnabled && ((Control)child).IsVisible) return (Control)child;
            if (FindFirstFocusable(child) is { } c) return c;
        }
        return null;
    }

    void Scrim_MouseDown(object sender, MouseButtonEventArgs e) => Vm.TutupDialog();

    // Esc closes the dialog; Ctrl+N opens the page's main "add" dialog when it has one.
    void OnKey(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.Escape && Vm.Dialog is not null) { Vm.TutupDialog(); e.Handled = true; return; }
        if (e.Key == Key.N && Keyboard.Modifiers == ModifierKeys.Control && Vm.Dialog is null && Vm.Halaman is IPunyaTambah p && p.TambahCommand.CanExecute(null))
        {
            p.TambahCommand.Execute(null);
            e.Handled = true;
        }
    }
}
