using System.Windows;
using System.Windows.Media;

namespace TernakKu.App;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        StateChanged += (_, _) => SyncState();
        Loaded += (_, _) => SyncState();
    }

    // A maximized borderless window overhangs the screen by the resize border; pad it back in.
    void SyncState()
    {
        var max = WindowState == WindowState.Maximized;
        root.Margin = max ? new Thickness(7) : new Thickness(0);
        maxIcon.Data = Geometry.Parse(max ? "M2.5,2.5 H9.5 V9.5 H2.5 Z M2.5,2.5 V0.5 H9.5 V7.5" : "M0.5,0.5 H9.5 V9.5 H0.5 Z");
        btnMax.ToolTip = max ? "Pulihkan ukuran" : "Maksimalkan";
    }

    void Minimize_Click(object sender, RoutedEventArgs e) => WindowState = WindowState.Minimized;

    void Maximize_Click(object sender, RoutedEventArgs e) =>
        WindowState = WindowState == WindowState.Maximized ? WindowState.Normal : WindowState.Maximized;

    void Close_Click(object sender, RoutedEventArgs e) => Close();
}
