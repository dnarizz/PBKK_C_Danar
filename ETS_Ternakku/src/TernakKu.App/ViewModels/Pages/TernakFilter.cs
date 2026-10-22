using TernakKu.Core.Models;

namespace TernakKu.App.ViewModels.Pages;

// Optional starting filter when opening the animal list from another page.
// BukaTambah opens the "Tambah ternak" dialog right away (used by the dashboard button and Ctrl+N).
public record TernakFilter(int? KandangId = null, int? SpesiesId = null, StatusTernak? Status = null, string Cari = "", bool BukaTambah = false);
