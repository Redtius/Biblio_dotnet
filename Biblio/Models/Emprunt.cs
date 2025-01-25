using System.ComponentModel.DataAnnotations;

namespace Biblio.Models
{
    public class Emprunt
    {
        public int Id { get; set; } // Identifiant de l'emprunt

        [Required]
        public int LivreId { get; set; } // Référence au livre emprunté
        public Livre Livre { get; set; } = null!; // Le livre emprunté

        [Required]
        public string UtilisateurId { get; set; } = string.Empty; // Référence à l'utilisateur qui a emprunté
        public Utilisateur Utilisateur { get; set; } = null!; // L'utilisateur qui a emprunté

        public DateTime DateEmprunt { get; set; } // Date d'emprunt

        [Required]
        public DateTime DateRetourPrevue { get; set; } // Date prévue de retour

        public DateTime? DateRetourEffective { get; set; } // Date réelle de retour (null si pas encore retourné)

        public bool EnAttenteDeConfirmation { get; set; } = false;
        public bool EstRetourne => DateRetourEffective.HasValue && !EnAttenteDeConfirmation;
        [Range(0, double.MaxValue)]
        public double? FraisRetard { get; set; } // Frais de retard, si applicable
    }
}
