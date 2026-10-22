using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using TernakKu.App.ViewModels.Pages;

namespace TernakKu.App.Views.Pages;

public partial class TernakView : UserControl
{
    public TernakView() => InitializeComponent();

    void Row_Click(object sender, MouseButtonEventArgs e)
    {
        if (sender is DataGridRow { Item: TernakViewModel.Baris b } && DataContext is TernakViewModel vm)
            vm.BukaCommand.Execute(b.Id);
    }

    // The table never scrolls itself; hand the wheel to the page scroller.
    void Grid_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
    {
        if (e.Handled) return;
        e.Handled = true;
        var ev = new MouseWheelEventArgs(e.MouseDevice, e.Timestamp, e.Delta) { RoutedEvent = UIElement.MouseWheelEvent, Source = sender };
        ((UIElement)((FrameworkElement)sender).Parent).RaiseEvent(ev);
    }
}
