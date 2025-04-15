using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class Chat
{
    [Key]
    public int ChatId { get; set; }

    public bool ArGruppChat { get; set; } 
    public string? GroupName { get; set; } 

    public DateTime SkapadTid { get; set; } = DateTime.UtcNow;
    public ICollection<Meddelande> Meddelanden { get; set; }
}