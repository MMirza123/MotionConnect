using System;

public class Notis
{
    public int NotisId { get; set; }
    public string Meddealande { get; set; }
    public bool ArLast { get; set; }
    public DateTime SkapadesTid { get; set; }

    public string AnvandarId { get; set; }
    public ApplicationUser Anvandare { get; set; }
}