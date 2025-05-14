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

    // Denna egenskap representerar mottagarens ID
    [Required]
    public string AnvandarId { get; set; }
    public ApplicationUser Anvandare { get; set; }

    [Required]
    public NotisTyp Typ { get; set; }
    
    public int? InlaggId { get; set; }
    public Inlagg Inlagg { get; set; }

    public int? MeddelandeId { get; set; }

    // Ny egenskap: avsändarens ID
    [Required]
    public string AvsandareId { get; set; }
    // Navigeringsegenskap för avsändaren
    public ApplicationUser Avsandare { get; set; }
}

