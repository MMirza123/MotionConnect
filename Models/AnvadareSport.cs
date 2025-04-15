using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class AnvandareSport
{
    public string AnvandarId { get; set; }
    public ApplicationUser Anvandare { get; set; }

    public int SportId { get; set; }
    public Sport Sport { get; set; }
}
