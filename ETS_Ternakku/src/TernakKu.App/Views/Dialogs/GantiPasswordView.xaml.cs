using System.Windows;
using System.Windows.Controls;
using TernakKu.App.ViewModels.Dialogs;

namespace TernakKu.App.Views.Dialogs;

public partial class GantiPasswordView : UserControl
{
    public GantiPasswordView() => InitializeComponent();

    void Lama_Changed(object sender, RoutedEventArgs e)
    {
        if (DataContext is GantiPasswordViewModel vm) vm.Lama = ((PasswordBox)sender).Password;
    }

    void Baru_Changed(object sender, RoutedEventArgs e)
    {
        if (DataContext is GantiPasswordViewModel vm) vm.Baru = ((PasswordBox)sender).Password;
    }

    void Ulangi_Changed(object sender, RoutedEventArgs e)
    {
        if (DataContext is GantiPasswordViewModel vm) vm.Ulangi = ((PasswordBox)sender).Password;
    }
}
