using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class Inlagg
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int InlaggId { get; set; }
    [Required]
    public string Text { get; set; }
    public string? BildUrl { get; set; }
    public DateTime SkapadesTid { get; set; } = DateTime.UtcNow;

    public string AnvandarId { get; set; }
    public ApplicationUser Anvandare { get; set; }

    public ICollection<Kommentar> Kommentarer { get; set; }
    public ICollection<Gillning> Gillningar { get; set; }
    public ICollection<InlaggSport> InlaggSporter { get; set; }
}