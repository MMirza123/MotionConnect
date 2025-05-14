using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MotionConnect.Models;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

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
    public DbSet<VanForFragan> Vanforfrågningar { get; set; }


    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

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

        builder.Entity<Inlagg>()
            .Property(i => i.InlaggId)
            .ValueGeneratedOnAdd();

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

        builder.Entity<VanForFragan>()
        .HasOne(v => v.Avsandare)
        .WithMany()
        .HasForeignKey(v => v.AvsandareId)
        .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<VanForFragan>()
            .HasOne(v => v.Mottagare)
            .WithMany()
            .HasForeignKey(v => v.MottagareId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<GruppMedlem>()
            .HasKey(gm => new { gm.GruppId, gm.AnvandarId });

        builder.Entity<GruppMedlem>()
            .HasOne(gm => gm.Grupp)
            .WithMany(g => g.GruppMedlemmar)
            .HasForeignKey(gm => gm.GruppId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<GruppMedlem>()
            .HasOne(gm => gm.Anvandare)
            .WithMany(u => u.GruppMedlemskap)
            .HasForeignKey(gm => gm.AnvandarId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<Kommentar>()
            .HasOne(c => c.Inlagg)
            .WithMany(p => p.Kommentarer)
            .HasForeignKey(c => c.InlaggId);

        builder.Entity<Gillning>()
            .HasOne(l => l.Inlagg)
            .WithMany(p => p.Gillningar)
            .HasForeignKey(l => l.InlaggId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<Gillning>()
            .HasOne(l => l.Kommentar)
            .WithMany(c => c.Gillningar)
            .HasForeignKey(l => l.KommentarId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<Notis>()
            .HasOne(n => n.Anvandare) // Mottagaren av notisen
            .WithMany(u => u.Notiser)
            .HasForeignKey(n => n.AnvandarId)
            .OnDelete(DeleteBehavior.Restrict); // Förhindrar kaskadborttagning

        builder.Entity<Notis>()
            .HasOne(n => n.Avsandare) // Avsändaren av notisen
            .WithMany()
            .HasForeignKey(n => n.AvsandareId)
            .OnDelete(DeleteBehavior.Restrict); // Förhindrar multipla cascade paths

        builder.Entity<Meddelande>()
            .HasOne(m => m.Avsandare)
            .WithMany()
            .HasForeignKey(m => m.AvsandareId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.Entity<Meddelande>()
            .HasOne(m => m.Chat)
            .WithMany(c => c.Meddelanden)
            .HasForeignKey(m => m.ChatId)
            .OnDelete(DeleteBehavior.Cascade);

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