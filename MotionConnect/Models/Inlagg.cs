using System;
using System.Collections.Generic;

public class Inlagg
{
    public int InlaggId { get; set; }
    public string Text { get; set; }
    public string BildUrl { get; set; }
    public DateTime SkapadesTid { get; set; } = DateTime.UtcNow;

    public string AnvandarId { get; set; }
    public ApplicationUser Anvandare { get; set; }

    public ICollection<Kommentar> Kommentarer { get; set; }
    public ICollection<Gilning> Gilningar { get; set; }
    public ICollection<InlaggSport> InlaggSporter { get; set; }
}