using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MotionConnect.Models; 

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) {}

     public ApplicationDbContext() { }

    public DbSet<Inlagg> Inlagg { get; set; }
    public DbSet<Sport> Sporter { get; set; }
    public DbSet<AnvandareSport> AnvandareSporter { get; set; }
    public DbSet<InlaggSport> InlaggSporter { get; set; }
    public DbSet<Van> Vanner { get; set; }
    public DbSet<Grupp> Grupper { get; set; }
    public DbSet<GruppMedlem> GruppMedlemmar { get; set; }
    public DbSet<Kommentar> Kommentarer { get; set; }
    public DbSet<Gillning> Gillningar { get; set; }
    public DbSet<Meddelande> Meddelanden { get; set; }
    public DbSet<Notis> Notiser { get; set; }
    public DbSet<Chat> Chattar { get; set; }
    public DbSet<MeddelandeMottagare> MeddelandeMottagare { get; set; }

   protected override void OnModelCreating(ModelBuilder builder)
{
    base.OnModelCreating(builder); // 👈 Viktigt för att ASP.NET Identity ska fungera

    // 🏀 Användare ↔ Sporter (M:M via AnvandareSporter)
    builder.Entity<AnvandareSport>()
        .HasKey(us => new { us.AnvandarId, us.SportId });

    builder.Entity<AnvandareSport>()
        .HasOne(us => us.Anvandare)
        .WithMany(u => u.AnvandareSporter)
        .HasForeignKey(us => us.AnvandarId);

    builder.Entity<AnvandareSport>()
        .HasOne(us => us.Sport)
        .WithMany(s => s.AnvandareSporter)
        .HasForeignKey(us => us.SportId);

    // 🏆 Inlägg ↔ Sporter (M:M via InlaggSporter)
    builder.Entity<InlaggSport>()
        .HasKey(ps => new { ps.InlaggId, ps.SportId });

    builder.Entity<InlaggSport>()
        .HasOne(ps => ps.Inlagg)
        .WithMany(p => p.InlaggSporter)
        .HasForeignKey(ps => ps.InlaggId);

    builder.Entity<InlaggSport>()
        .HasOne(ps => ps.Sport)
        .WithMany(s => s.InlaggSporter)
        .HasForeignKey(ps => ps.SportId);

    // 👥 Användare ↔ Vänner (M:M via Vanner)
    builder.Entity<Van>()
        .HasKey(v => new { v.AnvandarId, v.VanId });

    builder.Entity<Van>()
        .HasOne(v => v.Anvandare)
        .WithMany(u => u.Vanner)
        .HasForeignKey(v => v.AnvandarId)
        .OnDelete(DeleteBehavior.Restrict);

    builder.Entity<Van>()
        .HasOne(v => v.VanAnvandare)
        .WithMany()
        .HasForeignKey(v => v.VanId)
        .OnDelete(DeleteBehavior.Restrict);

    // 👥 Användare ↔ Grupper (M:M via GruppMedlemmar)
    builder.Entity<GruppMedlem>()
        .HasKey(gm => new { gm.GruppId, gm.AnvandarId }); // RÄTTAD primärnyckel!

    builder.Entity<GruppMedlem>()
        .HasOne(gm => gm.Grupp)
        .WithMany(g => g.GruppMedlemmar)
        .HasForeignKey(gm => gm.GruppId)
        .OnDelete(DeleteBehavior.Cascade); // Om en grupp tas bort, tas medlemmarna bort

    builder.Entity<GruppMedlem>()
        .HasOne(gm => gm.Anvandare)
        .WithMany(u => u.GruppMedlemskap)
        .HasForeignKey(gm => gm.AnvandarId)
        .OnDelete(DeleteBehavior.Restrict); // Hindrar multiple cascade paths

    // 📝 Inlägg → Kommentarer (1:N)
    builder.Entity<Kommentar>()
        .HasOne(c => c.Inlagg)
        .WithMany(p => p.Kommentarer)
        .HasForeignKey(c => c.InlaggId);

    // 👍 Inlägg → Gillningar (1:N)
    builder.Entity<Gillning>()
        .HasOne(l => l.Inlagg)
        .WithMany(p => p.Gillningar)
        .HasForeignKey(l => l.InlaggId)
        .OnDelete(DeleteBehavior.Cascade);

    // 👍 Kommentarer → Gillningar (1:N)
    builder.Entity<Gillning>()
        .HasOne(l => l.Kommentar)
        .WithMany(c => c.Gillningar)
        .HasForeignKey(l => l.KommentarId)
        .OnDelete(DeleteBehavior.Restrict); 

    // 🔔 Användare → Notiser (1:N)
    builder.Entity<Notis>()
        .HasOne(n => n.Anvandare)
        .WithMany(u => u.Notiser)
        .HasForeignKey(n => n.AnvandarId);

    // 📩 Användare → Meddelanden (1:N)
    builder.Entity<Meddelande>()
        .HasOne(m => m.Avsandare)
        .WithMany()
        .HasForeignKey(m => m.AvsandareId)
        .OnDelete(DeleteBehavior.NoAction); // Rättar multiple cascade paths

    builder.Entity<Meddelande>()
        .HasOne(m => m.Chat)
        .WithMany(c => c.Meddelanden)
        .HasForeignKey(m => m.ChatId)
        .OnDelete(DeleteBehavior.Cascade); // Om en chatt tas bort, tas meddelanden bort

    builder.Entity<MeddelandeMottagare>()
    .HasKey(mm => mm.MeddelandeMottagreId);

    builder.Entity<MeddelandeMottagare>()
        .HasOne(mm => mm.Meddelande)
        .WithMany(m => m.Mottagare)
        .HasForeignKey(mm => mm.MeddelandeId)
        .OnDelete(DeleteBehavior.Cascade);

    builder.Entity<MeddelandeMottagare>()
        .HasOne(mm => mm.Mottagare)
        .WithMany()
        .HasForeignKey(mm => mm.MottagareId)
        .OnDelete(DeleteBehavior.Restrict);

}

}