public class Gilningar
{
    public int GilingsId { get; set; }
    public int? InlaggId { get; set; }
    public Inlagg Inlagg { get; set; }

    public int? KommentarId { get; set; }
    public Kommentar kommentarer { get; set; }

    public string AnvandarId { get; set; }
    public ApplicationUser Anvandare { get; set; }
}