using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class Gillning
{
    [Key]
    public int GillingsId { get; set; }
    public int? InlaggId { get; set; }
    public Inlagg Inlagg { get; set; }

    public int? KommentarId { get; set; }
    public Kommentar Kommentar { get; set; }

    public string AnvandarId { get; set; }
    public ApplicationUser Anvandare { get; set; }
}