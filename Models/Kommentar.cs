using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class Kommentar
{
    public int KommentarId { get; set; }
    public string Text { get; set; }
    public DateTime SkapadTid { get; set; }

    public int InlaggId { get; set; }
    public Inlagg Inlagg { get; set; }

    public string AnvandarId { get; set; }
    public ApplicationUser Anvandare { get; set; }

    public ICollection<Gillning> Gillningar { get; set; }
}