using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Biblio.Data;
using Biblio.Models;

namespace Biblio.Controllers
{
    public class RechercheController : Controller
    {
        private readonly ApplicationDbContext _context;

        public RechercheController(ApplicationDbContext context)
        {
            _context = context;
        }

        // ✅ Page de recherche
        [HttpGet]
        public IActionResult Index()
        {
            return View(new RechercheLivre());
        }

        // ✅ Résultats de la recherche
        [HttpPost]
        public async Task<IActionResult> Resultats(RechercheLivre model)
        {
            if (model == null)
            {
                return View("Index", new RechercheLivre());
            }

            // ✅ Construction de la requête dynamique
            var query = _context.Livres.AsQueryable();

            if (!string.IsNullOrWhiteSpace(model.Titre))
            {
                query = query.Where(l => EF.Functions.Like(l.Titre, $"%{model.Titre}%"));
            }

            if (!string.IsNullOrWhiteSpace(model.Auteur))
            {
                query = query.Where(l => EF.Functions.Like(l.Auteur, $"%{model.Auteur}%"));
            }

            if (!string.IsNullOrWhiteSpace(model.Genre))
            {
                query = query.Where(l => EF.Functions.Like(l.Genre, $"%{model.Genre}%"));
            }

            if (model.AnneePublication.HasValue)
            {
                query = query.Where(l => l.AnneePublication == model.AnneePublication.Value);
            }

            if (model.DisponiblesUniquement.HasValue && model.DisponiblesUniquement.Value)
            {
                query = query.Where(l => l.CopiesDisponibles > 0);
            }

            model.ResultatsRecherche = await query.ToListAsync();

            return View("Index", model);
        }
    }
}

