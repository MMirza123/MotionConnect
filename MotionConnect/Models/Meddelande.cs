using System;

public class Meddelande
{
    public int MeddelandeId { get; set; }
    public string Text { get; set; }
    public DateTime SkapadesTid { get; set; }

    public string sandarId { get; set; }
    public ApplicationUser Sandare { get; set; }

    public string mottagarId { get; set; }
    public ApplicationUser Mottagare { get; set; }
}