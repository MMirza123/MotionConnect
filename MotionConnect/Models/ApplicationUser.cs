using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
public class ApplicationUser : IdentityUser
{
    public string ForNamn { get; set; }
    public string EfterNamn { get; set; }
    public string ProfilBildUrl { get; set; }
    public DateTime FodelsAr { get; set; }
    public bool ArProfilOppen { get; set; }

    public ICollection<Inlagg> Inlagg { get; set; }
    public ICollection<Kommentar> Kommentarer { get; set; }
    public ICollection<Meddelande> Meddelanden { get; set; }
    public ICollection<Notis> Notiser { get; set; }
    public ICollection<Gillning> Gillningar { get; set; }

    public ICollection<AnvandareSport> AnvandareSporter { get; set; }
    public ICollection<Van> Vanner { get; set; }
    public ICollection<GruppMedlem> GruppMedlemskap { get; set; }

}