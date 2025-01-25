using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Biblio.Data;
using Biblio.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace Biblio.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<Utilisateur> _userManager;

        public AdminController(ApplicationDbContext context, UserManager<Utilisateur> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // ✅ 1. Tableau de bord
        [HttpGet]
        public IActionResult Dashboard()
        {
            ViewBag.TotalLivres = _context.Livres.Count();
            ViewBag.TotalUtilisateurs = _userManager.Users.Count();
            ViewBag.EmpruntsEnCours = _context.Emprunts.Count(e => e.DateRetourEffective == null);

            return View();
        }

        // ✅ 2. Gérer les utilisateurs
        [HttpGet]
        public IActionResult GererUtilisateurs()
        {
            var utilisateurs = _userManager.Users.ToList();
            return View(utilisateurs);
        }

        // ✅ 3. Modifier un utilisateur
        [HttpGet]
        public async Task<IActionResult> EditUser(string id)
        {
            var utilisateur = await _userManager.FindByIdAsync(id);
            if (utilisateur == null)
            {
                return NotFound();
            }

            return View(utilisateur);
        }

        [HttpPost]
        public async Task<IActionResult> EditUser(Utilisateur utilisateur)
        {
            var userInDb = await _userManager.FindByIdAsync(utilisateur.Id);
            if (userInDb == null)
            {
                return NotFound();
            }

            userInDb.UserName = utilisateur.UserName;
            userInDb.Email = utilisateur.Email;
            userInDb.Telephone = utilisateur.Telephone;

            await _userManager.UpdateAsync(userInDb);
            return RedirectToAction("GererUtilisateurs");
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteUserConfirmed(string id)
        {
            var utilisateur = await _userManager.FindByIdAsync(id);
            if (utilisateur == null)
            {
                return NotFound();
            }

            await _userManager.DeleteAsync(utilisateur);
            return RedirectToAction("GererUtilisateurs");
        }

        // ✅ 5. Suivi des emprunts
        [HttpGet]
        public IActionResult SuiviEmprunts()
        {
            var emprunts = _context.Emprunts
                .Include(e => e.Livre)
                .Include(e => e.Utilisateur) 
                .OrderByDescending(e => e.DateEmprunt)
                .ToList();

            Console.WriteLine($"Nombre d'emprunts récupérés : {emprunts.Count}");

            foreach (var emprunt in emprunts)
            {
                Console.WriteLine($"Emprunt ID: {emprunt.Id}, Livre: {emprunt.Livre?.Titre ?? "❌ Livre manquant"}, Utilisateur: {emprunt.Utilisateur?.UserName ?? "❌ Utilisateur manquant"}");
            }

            return View(emprunts);
        }
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ConfirmerRetour(int id)
        {
            var emprunt = await _context.Emprunts
                .Include(e => e.Livre)
                .Include(e => e.Utilisateur)
                .FirstOrDefaultAsync(e => e.Id == id);

            if (emprunt == null || !emprunt.EnAttenteDeConfirmation)
            {
                return NotFound();
            }

            emprunt.DateRetourEffective = DateTime.Now;
            emprunt.EnAttenteDeConfirmation = false;
            emprunt.Livre.CopiesDisponibles += 1;

            // ✅ Calcul des frais de retard
            double fraisRetard = 0;
            if (emprunt.DateRetourEffective > emprunt.DateRetourPrevue)
            {
                var joursRetard = (emprunt.DateRetourEffective - emprunt.DateRetourPrevue).Value.Days;
                fraisRetard = joursRetard * 1.5; // Exemple : 1.5€ par jour de retard
            }

            // ✅ Convertir en chaîne pour TempData
            TempData["FraisRetard"] = fraisRetard.ToString("F2"); // Format avec 2 décimales
            TempData["EmpruntId"] = emprunt.Id.ToString();

            await _context.SaveChangesAsync();

            return RedirectToAction("SuiviEmprunts");
        }



        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AnnulerRetour(int id)
        {
            var emprunt = await _context.Emprunts.FindAsync(id);

            if (emprunt == null || !emprunt.EnAttenteDeConfirmation)
            {
                return NotFound();
            }

            emprunt.EnAttenteDeConfirmation = false;

            await _context.SaveChangesAsync();

            return RedirectToAction("SuiviEmprunts");
        }
    }
}
