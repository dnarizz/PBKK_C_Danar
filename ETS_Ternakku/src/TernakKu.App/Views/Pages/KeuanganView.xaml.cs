using System.Globalization;
using System.Windows.Controls;
using System.Windows.Data;

namespace TernakKu.App.Views.Pages;

public partial class KeuanganView : UserControl
{
    public KeuanganView() => InitializeComponent();
}

// Radio button <-> string option: checked when the bound text equals the parameter.
public class PilihanKonverter : IValueConverter
{
    public object Convert(object? value, Type t, object parameter, CultureInfo c) => string.Equals(value?.ToString(), parameter?.ToString(), StringComparison.Ordinal);
    public object ConvertBack(object value, Type t, object parameter, CultureInfo c) => value is true ? parameter.ToString()! : Binding.DoNothing;
}
