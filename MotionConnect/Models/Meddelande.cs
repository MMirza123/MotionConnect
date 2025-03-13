using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class Meddelande
{
    public int MeddelandeId { get; set; }

    public string Text { get; set; }
    public DateTime SkapadesTid { get; set; } = DateTime.UtcNow;

    public string AvsandareId { get; set; }
    public ApplicationUser Avsandare { get; set; }

    public int ChatId { get; set; }
    public Chat Chat { get; set; }

     public ICollection<MeddelandeMottagare> Mottagare { get; set; }
}