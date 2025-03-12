using System.Collections.Generic;

public class Sporter
{
    public int SportId { get; set; }
    public string Namn { get; set; }

    public ICollection<AnvandareSport> AnvandareSporter { get; set; }
    public ICollection<InlaggSport> InlaggSporter { get; set; }
}