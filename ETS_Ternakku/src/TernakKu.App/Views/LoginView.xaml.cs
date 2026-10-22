using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using TernakKu.App.ViewModels;

namespace TernakKu.App.Views;

public partial class LoginView : UserControl
{
    bool showPw;

    public LoginView()
    {
        InitializeComponent();
        DataContextChanged += (_, e) =>
        {
            if (e.OldValue is LoginViewModel old) old.IsiPassword -= Isi;
            if (e.NewValue is LoginViewModel vm) vm.IsiPassword += Isi;
        };
        Loaded += (_, _) =>
        {
            txtUser.Focus();
            Animasi.Masuk(form, formShift, 12, vertical: true);
        };
    }

    LoginViewModel Vm => (LoginViewModel)DataContext;

    void Isi(string password)
    {
        pwd.Password = password;
        pwdPlain.Text = password;
        pwd.Focus();
    }

    void Masuk_Click(object sender, RoutedEventArgs e)
    {
        Vm.Masuk(showPw ? pwdPlain.Text : pwd.Password);
        if (Vm.Error.Length > 0) { if (showPw) pwdPlain.Focus(); else pwd.Focus(); }
    }

    // PasswordBox cannot show its text, so swap in a TextBox while the eye is on.
    void Eye_Click(object sender, RoutedEventArgs e)
    {
        showPw = !showPw;
        if (showPw) { pwdPlain.Text = pwd.Password; pwdPlain.Visibility = Visibility.Visible; pwd.Visibility = Visibility.Collapsed; pwdPlain.Focus(); pwdPlain.CaretIndex = pwdPlain.Text.Length; }
        else { pwd.Password = pwdPlain.Text; pwd.Visibility = Visibility.Visible; pwdPlain.Visibility = Visibility.Collapsed; pwd.Focus(); }
        eyeIcon.Kind = showPw ? "eyeOff" : "eye";
        btnEye.ToolTip = showPw ? "Sembunyikan password" : "Tampilkan password";
    }

    void Creds_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
    {
        if (e.NewValue is true) Animasi.Masuk((FrameworkElement)sender, credsShift, -8, vertical: false);
    }
}

// Shared entrance animation: fade in while sliding into place (250 ms, decelerate).
public static class Animasi
{
    public static void Masuk(FrameworkElement el, System.Windows.Media.TranslateTransform shift, double dari, bool vertical)
    {
        if (!SystemParameters.ClientAreaAnimation) return;
        var dur = TimeSpan.FromMilliseconds(250);
        var ease = new ExponentialEase { EasingMode = EasingMode.EaseOut, Exponent = 6 };
        el.BeginAnimation(UIElement.OpacityProperty, new DoubleAnimation(0, 1, dur) { EasingFunction = ease });
        shift.BeginAnimation(vertical ? System.Windows.Media.TranslateTransform.YProperty : System.Windows.Media.TranslateTransform.XProperty,
            new DoubleAnimation(dari, 0, dur) { EasingFunction = ease });
    }
}
