using System.Windows.Controls;
using System.Windows.Input;
using TernakKu.App.ViewModels.Pages;

namespace TernakKu.App.Views.Pages;

public partial class DashboardView : UserControl
{
    public DashboardView() => InitializeComponent();

    // Enter in the search box opens the animal list filtered by the text.
    void Cari_KeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.Enter && DataContext is DashboardViewModel vm) vm.Cari(cari.Text);
    }
}
