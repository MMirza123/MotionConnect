using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class Sport
{
    public int SportId { get; set; }
    public string Namn { get; set; }

    public ICollection<AnvandareSport> AnvandareSporter { get; set; }
    public ICollection<InlaggSport> InlaggSporter { get; set; }
}