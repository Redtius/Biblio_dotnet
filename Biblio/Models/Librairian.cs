namespace Biblio.Models
{
    using System;
    using System.Collections.Generic;

    public class Bibliothecaire : Utilisateur
    {
        // Relation avec les livres gérés par ce bibliothécaire
        public List<Livre> LivresGeres { get; set; } = new List<Livre>();
    }


}
