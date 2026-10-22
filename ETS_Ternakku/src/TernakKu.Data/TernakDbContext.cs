using Microsoft.EntityFrameworkCore;
using TernakKu.Core.Models;

namespace TernakKu.Data;

public class TernakDbContext(DbContextOptions<TernakDbContext> options) : DbContext(options)
{
    public DbSet<Pengguna> Pengguna => Set<Pengguna>();
    public DbSet<Spesies> Spesies => Set<Spesies>();
    public DbSet<Ras> Ras => Set<Ras>();
    public DbSet<Kandang> Kandang => Set<Kandang>();
    public DbSet<Ternak> Ternak => Set<Ternak>();
    public DbSet<Penimbangan> Penimbangan => Set<Penimbangan>();
    public DbSet<Pakan> Pakan => Set<Pakan>();
    public DbSet<PakanMasuk> PakanMasuk => Set<PakanMasuk>();
    public DbSet<PemberianPakan> PemberianPakan => Set<PemberianPakan>();
    public DbSet<ObatVaksin> ObatVaksin => Set<ObatVaksin>();
    public DbSet<Kesehatan> Kesehatan => Set<Kesehatan>();
    public DbSet<JadwalVaksinasi> JadwalVaksinasi => Set<JadwalVaksinasi>();
    public DbSet<Perkawinan> Perkawinan => Set<Perkawinan>();
    public DbSet<Kelahiran> Kelahiran => Set<Kelahiran>();
    public DbSet<Mutasi> Mutasi => Set<Mutasi>();
    public DbSet<Keuangan> Keuangan => Set<Keuangan>();
    public DbSet<TugasSelesai> TugasSelesai => Set<TugasSelesai>();

    protected override void OnModelCreating(ModelBuilder b)
    {
        b.Entity<Pengguna>().HasIndex(x => x.Username).IsUnique();
        b.Entity<Kandang>().HasIndex(x => x.Kode).IsUnique();
        b.Entity<Kandang>().Ignore(x => x.Label);

        b.Entity<Ternak>(e =>
        {
            e.HasIndex(x => x.KodeTernak).IsUnique();
            e.Ignore(x => x.Label);
            e.HasOne(x => x.Induk).WithMany().HasForeignKey(x => x.IndukId).OnDelete(DeleteBehavior.Restrict);
            e.HasOne(x => x.Pejantan).WithMany().HasForeignKey(x => x.PejantanId).OnDelete(DeleteBehavior.Restrict);
            e.HasOne(x => x.Kandang).WithMany(k => k.Ternak).HasForeignKey(x => x.KandangId).OnDelete(DeleteBehavior.Restrict);
            e.HasOne(x => x.Ras).WithMany().HasForeignKey(x => x.RasId).OnDelete(DeleteBehavior.Restrict);
        });

        b.Entity<Penimbangan>().HasIndex(x => new { x.TernakId, x.Tanggal });
        b.Entity<JadwalVaksinasi>().HasIndex(x => new { x.TanggalRencana, x.Status });
        b.Entity<Keuangan>().HasIndex(x => x.Tanggal);

        b.Entity<Perkawinan>(e =>
        {
            e.HasOne(x => x.Induk).WithMany().HasForeignKey(x => x.IndukId).OnDelete(DeleteBehavior.Restrict);
            e.HasOne(x => x.Pejantan).WithMany().HasForeignKey(x => x.PejantanId).OnDelete(DeleteBehavior.Restrict);
            e.HasOne(x => x.Kelahiran).WithOne(k => k.Perkawinan).HasForeignKey<Kelahiran>(k => k.PerkawinanId);
        });

        b.Entity<Mutasi>().HasOne(x => x.Ternak).WithMany().HasForeignKey(x => x.TernakId).OnDelete(DeleteBehavior.Restrict);
    }
}
