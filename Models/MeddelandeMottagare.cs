using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class MeddelandeMottagare
{
    [Key]
    public int MeddelandeMottagreId { get; set; }
    public int MeddelandeId { get; set; }
    public Meddelande Meddelande { get; set; }

    public string MottagareId { get; set; }
    public ApplicationUser Mottagare { get; set; }
}
