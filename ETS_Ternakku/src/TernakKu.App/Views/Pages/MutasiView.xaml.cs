using System.Windows.Controls;
using System.Windows.Input;
using TernakKu.App.ViewModels.Pages;

namespace TernakKu.App.Views.Pages;

public partial class MutasiView : UserControl
{
    public MutasiView() => InitializeComponent();

    void Row_Click(object sender, MouseButtonEventArgs e)
    {
        if (sender is DataGridRow { Item: MutasiViewModel.Baris b } && DataContext is MutasiViewModel vm)
            vm.BukaCommand.Execute(b.TernakId);
    }
}
