using System.Windows;
using System.Windows.Controls;
using TernakKu.App.ViewModels.Pages;

namespace TernakKu.App.Views.Pages;

public partial class LaporanView : UserControl
{
    public LaporanView() => InitializeComponent();

    // The user can pick "Microsoft Print to PDF" in the print dialog to get a PDF file.
    void SimpanPdf_Click(object sender, RoutedEventArgs e)
    {
        var dlg = new PrintDialog();
        if (dlg.ShowDialog() != true) return;
        var judul = (DataContext as LaporanViewModel)?.JudulLaporan ?? "Laporan TernakKu";
        dlg.PrintVisual(paper, judul);
        (DataContext as LaporanViewModel)?.SelesaiCetak();
    }
}
