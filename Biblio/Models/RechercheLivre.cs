namespace Biblio.Models
{
    public class RechercheLivre
    {
        public string? Titre { get; set; } // Critère de recherche par titre
        public string? Auteur { get; set; } // Critère de recherche par auteur
        public string? Genre { get; set; } // Critère de recherche par genre
        public int? AnneePublication { get; set; } // Critère de recherche par année de publication
        public bool? DisponiblesUniquement { get; set; } // Filtrer uniquement les livres disponibles

        // Pour afficher les livres correspondant à cette recherche
        public List<Livre> ResultatsRecherche { get; set; } = new List<Livre>();
    }
}
