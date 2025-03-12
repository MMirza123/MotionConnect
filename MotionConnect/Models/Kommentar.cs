using System;

public class Kommentarer
{
    public int KommentarId { get; set; }
    public string Kommentar { get; set; }
    public DateTime SkapadTid { get; set; }

    public int InlaggId { get; set; }
    public Inlagg Inlagg { get; set; }

    public string AnvandarId { get; set; }
    public ApplicationUser Anvandare { get; set; }
}