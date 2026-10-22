using System.Globalization;
using System.Windows;
using System.Windows.Markup;
using TernakKu.App.Services;
using TernakKu.App.ViewModels;

namespace TernakKu.App;

public partial class App : Application
{
    protected override void OnStartup(StartupEventArgs e)
    {
        var id = CultureInfo.GetCultureInfo("id-ID");
        CultureInfo.DefaultThreadCurrentCulture = id;
        CultureInfo.DefaultThreadCurrentUICulture = id;
        FrameworkElement.LanguageProperty.OverrideMetadata(typeof(FrameworkElement), new FrameworkPropertyMetadata(XmlLanguage.GetLanguage(id.IetfLanguageTag)));

        base.OnStartup(e);
        ThemeService.Apply(Settings.Current.Theme);
        ApplyDensity(Settings.Current.PadatTabel);

        var main = new MainViewModel();
        var window = new MainWindow { DataContext = main };
        MainWindow = window;
        window.Show();
        main.Mulai();
    }

    public static void ApplyDensity(bool padat) => Current.Resources["RowHeight"] = padat ? 36.0 : 44.0;
}
