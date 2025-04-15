using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class GruppMedlem
{
    public int GruppMedlemId { get; set; }

    public int GruppId { get; set; }
    public Grupp Grupp { get; set; }
    public string AnvandarId { get; set; }
    public ApplicationUser Anvandare { get; set; } 
}
