using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class Chat
{
    [Key]
    public int ChatId { get; set; }

    public bool ArGruppChat { get; set; }  // True = Gruppchatt, False = Privat

    public string? GroupName { get; set; }  // Null om det är en privat chatt

    public DateTime SkapadTid { get; set; } = DateTime.UtcNow;

    // Relationer
    public ICollection<Meddelande> Meddelanden { get; set; }
}