using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class Van 
{
    public string AnvandarId { get; set; }
    public ApplicationUser Anvandare { get; set; }
    public string VanId { get; set; }
    public ApplicationUser VanAnvandare { get; set; }
}