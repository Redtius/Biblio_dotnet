using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Biblio.Models;
using Biblio.ViewModels;
using Microsoft.AspNetCore.Authorization;

namespace Biblio.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<Utilisateur> _userManager;
        private readonly SignInManager<Utilisateur> _signInManager;

        public AccountController(UserManager<Utilisateur> userManager, SignInManager<Utilisateur> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                Console.WriteLine("❌ User not found by email.");
                ModelState.AddModelError(string.Empty, "Email ou mot de passe incorrect.");
                return View(model);
            }

            Console.WriteLine($"✅ Found user: {user.UserName}");

            var result = await _signInManager.PasswordSignInAsync(user.UserName, model.Password, false, false);

            if (result.Succeeded)
            {
                Console.WriteLine("✅ Login successful.");
                return RedirectToAction("Index", "Home");
            }

            Console.WriteLine($"❌ Login failed: {result}");
            ModelState.AddModelError(string.Empty, "Email ou mot de passe incorrect.");
            return View(model);
        }



        // Déconnexion
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = new Utilisateur { UserName = model.Username, Email = model.Email, EmailConfirmed = true };
            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                await _signInManager.SignInAsync(user, isPersistent: false);
                return RedirectToAction("Index", "Home");
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return View(model);
        }

        // Profil utilisateur
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Profile()
        {
            var user = await _userManager.GetUserAsync(User);
            return View(user);
        }

        // Mise à jour du profil
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Profile(string telephone)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user != null)
            {
                user.Telephone = telephone;
                await _userManager.UpdateAsync(user);
                ViewBag.Message = "Profil mis à jour avec succès.";
            }
            else
            {
                ModelState.AddModelError("", "Erreur lors de la mise à jour du profil.");
            }

            return View(user);
        }
    }
}
