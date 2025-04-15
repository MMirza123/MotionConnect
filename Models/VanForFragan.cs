using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class VanForFragan 
{
    [Key]
    public int Id { get; set; }
    public string AvsandareId { get; set; }
    public ApplicationUser Avsandare { get; set; }

    public string MottagareId { get; set; }
    public ApplicationUser Mottagare { get; set; }
    public DateTime Skickades { get; set; } = DateTime.UtcNow;
    public bool ArGodkand { get; set; } = false;
}