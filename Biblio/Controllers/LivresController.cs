
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Biblio.Models;
using Biblio.Data;
using Microsoft.AspNetCore.Authorization;
namespace Biblio.Controllers
{

    public class LivreController : Controller
    {
        private readonly ApplicationDbContext _context;

        public LivreController(ApplicationDbContext context)
        {
            _context = context;
        }

        // ✅ Afficher la liste des livres
        [HttpGet]
        public IActionResult Index()
        {
            var livres = _context.Livres.ToList();
            return View(livres);
        }

        // ✅ Afficher les détails d'un livre
        [HttpGet]
        public IActionResult Details(int id)
        {
            var livre = _context.Livres.Find(id);
            if (livre == null)
            {
                return NotFound();
            }
            return View(livre);
        }

        // ✅ Afficher le formulaire d'ajout d'un livre (Admin uniquement)
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            return View();
        }

        // ✅ Traiter l'ajout d'un livre
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public IActionResult Create(Livre livre)
        {
            if (ModelState.IsValid)
            {
                _context.Livres.Add(livre);
                _context.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(livre);
        }

        // ✅ Afficher le formulaire de modification d'un livre (Admin uniquement)
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public IActionResult Edit(int id)
        {
            var livre = _context.Livres.Find(id);
            if (livre == null)
            {
                return NotFound();
            }
            return View(livre);
        }

        // ✅ Traiter la modification d'un livre
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public IActionResult Edit(int id, Livre livre)
        {
            if (id != livre.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                _context.Livres.Update(livre);
                _context.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(livre);
        }

        // ✅ Afficher la confirmation de suppression (Admin uniquement)
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public IActionResult Delete(int id)
        {
            var livre = _context.Livres.Find(id);
            if (livre == null)
            {
                return NotFound();
            }
            return View(livre);
        }

        // ✅ Traiter la suppression d'un livre
        [HttpPost, ActionName("Delete")]
        [Authorize(Roles = "Admin")]
        public IActionResult DeleteConfirmed(int id)
        {
            var livre = _context.Livres.Find(id);
            if (livre != null)
            {
                _context.Livres.Remove(livre);
                _context.SaveChanges();
            }
            return RedirectToAction("Index");
        }
    }

}
