using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class RegisterViewModel 
{
    [Required(ErrorMessage = "Förnamn är obligatoriskt")]
    public string Fornamn { get; set; }

    [Required(ErrorMessage = "Efternamn är obligatoriskt")]
    public string Efternamn { get; set; }

    [Required(ErrorMessage = "Email är obligatoriskt")]
    [EmailAddress(ErrorMessage = "Ogiltig e-postadress")]
    public string Email { get; set; }

    [Required(ErrorMessage = "Telefonnummer är obligatoriskt")]
    [RegularExpression(@"^\+?[0-9\s-]{7,15}$", ErrorMessage = "Ogiltigt telefonnummer")]
    public string Telefonnummer { get; set; }

    [Required(ErrorMessage = "Födelseår är obligatoriskt")]
    [DataType(DataType.Date)]
    public DateTime Fodelsear { get; set; }

    [Required(ErrorMessage = "Lösenord är obligatoriskt")]
    [DataType(DataType.Password)]
    public string Losenord { get; set; }

    [Required(ErrorMessage = "Bekräfta lösenord")]
    [DataType(DataType.Password)]
    [Compare("Losenord", ErrorMessage = "Lösenorden matchar inte")]
    public string BekraftaLosenord { get; set; }
    public bool ArProfilOppen { get; set; }
    public IFormFile? Profilbild { get; set; }

}