namespace TernakKu.App.ViewModels.Dialogs;

// Generic yes/no dialog. Simpan() runs the action; return false from it to keep the dialog open.
public partial class KonfirmasiViewModel(ShellViewModel shell, string judul, string pesan, string tombol, bool bahaya, Func<bool> aksi) : DialogViewModel(shell)
{
    public override string Judul => judul;
    public override string TombolOk => tombol;
    public override bool Bahaya => bahaya;
    public string Pesan => pesan;

    protected override bool Simpan() => aksi();
}
