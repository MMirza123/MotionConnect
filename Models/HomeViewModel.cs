using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class HomeViewModel
{
    public ApplicationUser Anvandare { get; set; }
    public List<ApplicationUser> Chattar { get; set; }
    public List<Inlagg> Inlagg { get; set; }
    public List<int?> HarGillatInlaggIds { get; set; } = new();
    public Dictionary<int?, int> AntalGillningar { get; set; } = new();
}
