using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class Notis
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int NotisId { get; set; }

    [Required]
    public string Meddelande { get; set; }

    public bool ArLast { get; set; } = false;

    public DateTime SkapadesTid { get; set; } = DateTime.UtcNow;

    [Required]
    public string AnvandarId { get; set; }
    public ApplicationUser Anvandare { get; set; }

    [Required]
    public NotisTyp Typ { get; set; }
    public int? InlaggId { get; set; }
    public Inlagg Inlagg { get; set; }

    public int? MeddelandeId { get; set; }
}

