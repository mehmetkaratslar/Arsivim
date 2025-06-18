using Microsoft.EntityFrameworkCore;
using Arsivim.Core.Models;
using Arsivim.Core.Enums;

namespace Arsivim.Data.Context
{
    public class ArsivimContext : DbContext
    {
        public ArsivimContext(DbContextOptions<ArsivimContext> options) : base(options)
        {
        }

        // DbSet tanımları
        public DbSet<Kisi> Kisiler { get; set; }
        public DbSet<Belge> Belgeler { get; set; }
        public DbSet<KisiBelge> KisiBelgeler { get; set; }
        public DbSet<Etiket> Etiketler { get; set; }
        public DbSet<BelgeEtiket> BelgeEtiketler { get; set; }
        public DbSet<BelgeVersiyon> BelgeVersiyonlari { get; set; }
        public DbSet<Not> Notlar { get; set; }
        public DbSet<Favori> Favoriler { get; set; }
        public DbSet<Gecmis> Gecmis { get; set; }
        public DbSet<Ayarlar> Ayarlar { get; set; }
        public DbSet<Kullanici> Kullanicilar { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // KisiBelge Many-to-Many ilişkisi
            modelBuilder.Entity<KisiBelge>()
                .HasKey(kb => new { kb.KisiID, kb.BelgeID });

            modelBuilder.Entity<KisiBelge>()
                .HasOne(kb => kb.Kisi)
                .WithMany(k => k.KisiBelgeleri)
                .HasForeignKey(kb => kb.KisiID);

            modelBuilder.Entity<KisiBelge>()
                .HasOne(kb => kb.Belge)
                .WithMany(b => b.KisiBelgeleri)
                .HasForeignKey(kb => kb.BelgeID);

            // BelgeEtiket Many-to-Many ilişkisi
            modelBuilder.Entity<BelgeEtiket>()
                .HasKey(be => new { be.BelgeID, be.EtiketID });

            // Enum conversion
            modelBuilder.Entity<Belge>()
                .Property(b => b.BelgeTipi)
                .HasConversion<int>();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                var dbPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Arsivim", "arsivim.db");
                Directory.CreateDirectory(Path.GetDirectoryName(dbPath)!);
                optionsBuilder.UseSqlite($"Data Source={dbPath}");
            }
        }
    }
} 