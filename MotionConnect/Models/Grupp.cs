using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class Grupp
{
    public int GruppId { get; set; }
    public string GruppNamn { get; set; }

    public DateTime SkapadesTid { get; set; } = DateTime.UtcNow;

    // Relation till GruppMedlem (M:M mellan Användare och Grupper)
    public ICollection<GruppMedlem> GruppMedlemmar { get; set; }
}
