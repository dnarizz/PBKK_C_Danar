using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace TernakKu.App.Views.Pages;

public partial class PakanView : UserControl
{
    public PakanView() => InitializeComponent();

    // Quick-fill chips act like buttons, not toggles.
    void Chip_Click(object sender, RoutedEventArgs e) => ((ToggleButton)sender).IsChecked = false;
}
