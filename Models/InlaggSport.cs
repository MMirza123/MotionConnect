using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class InlaggSport
{
    public int InlaggId { get; set; }
    public Inlagg Inlagg { get; set; }

    public int SportId { get; set; }
    public Sport Sport { get; set; }
}
