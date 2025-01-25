using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Biblio.Models;
using Biblio.Data;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace Biblio.Controllers
{
    [Authorize]
    public class EmpruntController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<Utilisateur> _userManager;

        public EmpruntController(ApplicationDbContext context, UserManager<Utilisateur> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // ✅ Liste des emprunts
        [HttpGet]
        public IActionResult Index()
        {
            var emprunts = _context.Emprunts
                .Include(e => e.Livre)
                .Include(e => e.Utilisateur)
                .Where(e => e.UtilisateurId == _userManager.GetUserId(User))
                .ToList();

            return View(emprunts);
        }

        // ✅ Afficher le formulaire pour emprunter un livre
        [HttpGet]
        public IActionResult Create()
        {
            ViewBag.Livres = _context.Livres.Where(l => l.CopiesDisponibles > 0).ToList();
            return View();
        }

        // ✅ Traiter l'emprunt d'un livre
        [HttpPost]
        public IActionResult Create(int LivreId, DateTime DateRetourPrevue)
        {
            var livre = _context.Livres.Find(LivreId);
            if (livre == null || livre.CopiesDisponibles <= 0)
            {
                ModelState.AddModelError("", "Ce livre n'est pas disponible.");
                return View();
            }

            var emprunt = new Emprunt
            {
                LivreId = LivreId,
                UtilisateurId = _userManager.GetUserId(User),
                DateEmprunt = DateTime.Now,
                DateRetourPrevue = DateRetourPrevue
            };

            livre.CopiesDisponibles--;
            _context.Emprunts.Add(emprunt);
            _context.SaveChanges();

            return RedirectToAction("Index");
        }

        // ✅ Afficher la confirmation de retour
        [HttpGet]
        public IActionResult Retour(int id)
        {
            var emprunt = _context.Emprunts.Find(id);
            if (emprunt == null || emprunt.UtilisateurId != _userManager.GetUserId(User))
            {
                return NotFound();
            }
            return View(emprunt);
        }

        // ✅ Traiter le retour d'un livre
        [HttpPost, ActionName("RetourConfirmed")]
        public IActionResult RetourConfirmed(int id)
        {
            var emprunt = _context.Emprunts.Find(id);
            if (emprunt == null || emprunt.UtilisateurId != _userManager.GetUserId(User))
            {
                return NotFound();
            }

            emprunt.DateRetourEffective = DateTime.Now;
            var livre = _context.Livres.Find(emprunt.LivreId);
            if (livre != null)
            {
                livre.CopiesDisponibles++;
            }

            _context.SaveChanges();
            return RedirectToAction("Index");
        }
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> SignalerRetour(int id)
        {
            var emprunt = await _context.Emprunts
                .Include(e => e.Livre)
                .FirstOrDefaultAsync(e => e.Id == id);

            if (emprunt == null || emprunt.EstRetourne || emprunt.EnAttenteDeConfirmation)
            {
                return NotFound();
            }

            emprunt.EnAttenteDeConfirmation = true;

            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }
    }

}
