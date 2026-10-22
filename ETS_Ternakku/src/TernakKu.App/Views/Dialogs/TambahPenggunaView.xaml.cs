using System.Windows;
using System.Windows.Controls;
using TernakKu.App.ViewModels.Dialogs;

namespace TernakKu.App.Views.Dialogs;

public partial class TambahPenggunaView : UserControl
{
    public TambahPenggunaView() => InitializeComponent();

    void Password_Changed(object sender, RoutedEventArgs e)
    {
        if (DataContext is TambahPenggunaViewModel vm) vm.Password = ((PasswordBox)sender).Password;
    }

    void Ulangi_Changed(object sender, RoutedEventArgs e)
    {
        if (DataContext is TambahPenggunaViewModel vm) vm.Ulangi = ((PasswordBox)sender).Password;
    }
}
