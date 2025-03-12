using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;

public class ApplicationUser : IdentityUser
{
    public string ForNamn { get; set; }
    public string EfterNamn { get; set; }
    public string ProfilBildUrl { get; set; }
    public DateTime FodelsAr { get; set; }
    public bool ArProfilOppen { get; set; }

    public ICollction<Inlagg> Inlagg { get; set; }
    public ICollction<Kommentar> Kommentarer { get; set; }
    public ICollction<Meddelande> Meddelanden { get; set; }
    public ICollction<Notis> Notiser { get; set; }
    public ICollction<Gilning> Gilningar { get; set; }

    public ICollction<AnvandareSport> AnvandareSporter { get; set; }
    public ICollction<Van> Vanner { get; set; }
    public ICollction<GruppMedlem> GruppMedlemar { get; set; }

}