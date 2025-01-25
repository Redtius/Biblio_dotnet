using Microsoft.AspNetCore.Mvc;

namespace Biblio.Controllers
{
    public class HomeController : Controller
    {
        // Page d'accueil
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        // Page � Propos
        [HttpGet]
        public IActionResult About()
        {
            return View();
        }

        // Page de Contact (Formulaire)
        [HttpGet]
        public IActionResult Contact()
        {
            return View();
        }

        // Traitement du formulaire de Contact
        [HttpPost]
        public IActionResult Contact(string nom, string email, string message)
        {
            if (string.IsNullOrEmpty(nom) || string.IsNullOrEmpty(email) || string.IsNullOrEmpty(message))
            {
                ModelState.AddModelError("", "Tous les champs sont requis.");
                return View();
            }

            // Ici, on peut ajouter une logique pour envoyer un email ou stocker le message en base de donn�es.
            ViewBag.Message = "Votre message a �t� envoy� avec succ�s.";
            return View();
        }

        // Page d'erreur
        [HttpGet]
        public IActionResult Error()
        {
            return View();
        }
    }
}
