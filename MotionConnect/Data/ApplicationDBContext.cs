using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) {}

    public DbSet<Inlagg> Inlagg { get; set; }
    public DbSet<Sport> Sporter { get; set; }
    public DbSet<AnvandareSport> AnvandareSporter { get; set; }
    public DbSet<InlaggSport> InlaggSporter { get; set; }
    public DbSet<Van> Vanner { get; set; }
    public DbSet<Grupp> Grupper { get; set; }
    public DbSet<GruppMedlem> GruppMedlemar { get; set; }
    public DbSet<Kommentar> Kommentarer { get; set; }
    public DbSet<Gilning> Gilningar { get; set; }
    public DbSet<Meddelande> Meddelanden { get; set; }
    public DbSet<Notis> Notiser { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<AnvandareSport>()
            .HasKey(us => new { us.AnvandarId, us.SportId});

    }
    
    
}