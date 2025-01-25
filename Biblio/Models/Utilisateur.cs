using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace Biblio.Models
{
    public class Utilisateur : IdentityUser
    {
        [Phone]
        public string? Telephone { get; set; } // Numéro de téléphone (facultatif)

        // Relation avec les emprunts effectués par l'utilisateur
        public List<Emprunt> Emprunts { get; set; } = new List<Emprunt>();
    }

}
