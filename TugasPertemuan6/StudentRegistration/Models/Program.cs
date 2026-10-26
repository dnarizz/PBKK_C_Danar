namespace StudentRegistration.Models;

// Model Program Studi (bukan file entry point aplikasi)
public class Program
{
    public int ProgramId { get; set; }

    public string ProgramCode { get; set; } = string.Empty;

    public string ProgramName { get; set; } = string.Empty;

    public bool IsActive { get; set; }
}
