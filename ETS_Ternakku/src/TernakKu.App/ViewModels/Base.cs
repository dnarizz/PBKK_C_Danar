using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace TernakKu.App.ViewModels;

// A screen shown inside the shell. Load() reads fresh data from the open database.
public abstract partial class PageViewModel(ShellViewModel shell) : ObservableObject
{
    protected ShellViewModel Shell { get; } = shell;
    public abstract string Key { get; }
    public virtual void Load() { }
}

// Pages with a main "add" action; Ctrl+N runs it.
public interface IPunyaTambah
{
    IRelayCommand TambahCommand { get; }
}

// A modal form shown over the shell. Simpan() returns true when the dialog may close.
public abstract partial class DialogViewModel(ShellViewModel shell) : ObservableObject
{
    protected ShellViewModel Shell { get; } = shell;
    public abstract string Judul { get; }
    public virtual string TombolOk => "Simpan";
    public virtual bool Bahaya => false;
    public virtual bool Lebar => false;

    protected abstract bool Simpan();

    [RelayCommand]
    void Ok()
    {
        if (Simpan()) Shell.TutupDialog();
    }

    [RelayCommand]
    void Batal() => Shell.TutupDialog();
}

public partial class SnackViewModel(string teks, Action? undo) : ObservableObject
{
    public string Teks { get; } = teks;
    public Action? Undo { get; } = undo;
    public bool BisaUndo => Undo is not null;
}
