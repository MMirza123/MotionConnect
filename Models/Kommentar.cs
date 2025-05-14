using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class Kommentar
{
    [Key]  // Markerar det här som primärnyckeln
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int KommentarId { get; set; }
    public string Text { get; set; }
    public DateTime SkapadTid { get; set; }

    public int InlaggId { get; set; }
    public Inlagg Inlagg { get; set; }

    public string AnvandareId { get; set; }
    public ApplicationUser Anvandare { get; set; }

    public ICollection<Gillning> Gillningar { get; set; }
}