using System.ComponentModel.DataAnnotations;

namespace Biblio.Models
{
    public class Livre
    {
        public int Id { get; set; } // Identifiant unique du livre

        [Required]
        [StringLength(200)]
        public string Titre { get; set; } = string.Empty; // Titre du livre

        [Required]
        [StringLength(100)]
        public string Auteur { get; set; } = string.Empty; // Auteur du livre

        [StringLength(100)]
        public string Editeur { get; set; } = string.Empty; // Éditeur du livre

        [StringLength(50)]
        public string Genre { get; set; } = string.Empty; // Genre du livre

        public int AnneePublication { get; set; } // Année de publication

        [Range(0, int.MaxValue)]
        public int CopiesDisponibles { get; set; } // Nombre de copies disponibles

        // Relation avec les emprunts
        public List<Emprunt> Emprunts { get; set; } = new List<Emprunt>();
    }

}
