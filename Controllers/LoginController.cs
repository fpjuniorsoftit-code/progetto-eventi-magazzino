
/* Questo controller ha 4 responsabilità:
 1) Mostrare la pagina di login.
 2) Recuperare l'utente dal database tramite username.
 3) Verificare la password con PasswordService.
 4) Creare il cookie con il ruolo corretto (Admin oppure User).
*/

using System.Security.Claims;
using GestionEventiMagazzino.Data;
using GestionEventiMagazzino.Services;
using GestionEventiMagazzino.ViewModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GestionEventiMagazzino.Controllers
{
    public class LoginController : Controller
    {
        private readonly ApplicationDbContext context;
        private readonly PasswordService passwordService;

        public LoginController(
            ApplicationDbContext context,
            PasswordService passwordService)
        {
            this.context = context;
            this.passwordService = passwordService;
        }

        // Visualizzazione login: questo metodo GET mostra la pagina di login
        [AllowAnonymous]
        [HttpGet]
        public IActionResult Index(string? returnUrl = null)
        {
            // Se l'utente è già autenticato, lo reindirizziamo in base al ruolo
            if (User.Identity?.IsAuthenticated == true)
            {
                if (User.IsInRole("Admin"))
                    return RedirectToAction("Index", "Admin");

                return RedirectToAction("Index", "Home");
            }

            return View(new LoginViewModel { ReturnUrl = returnUrl });
        }

        // Verifica credenziali: questo metodo POST si attiva quando l'utente clicca "Accedi"
        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(LoginViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var utente = await context.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.username == model.Username);

            if (utente == null ||
                !passwordService.VerifyPassword(utente, model.Password))
            {
                ModelState.AddModelError(
                    string.Empty,
                    "Nome utente o password non corretti.");

                return View(model);
            }

            // Creazione delle informazioni dell'utente per il cookie
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, utente.username),
                new Claim(ClaimTypes.Name, utente.username),
                new Claim(ClaimTypes.Role, utente.IsAdmin ? "Admin" : "User")
            };

            var identity = new ClaimsIdentity(
                claims,
                CookieAuthenticationDefaults.AuthenticationScheme);

            var principal = new ClaimsPrincipal(identity);

            // Emissione del cookie di autenticazione
            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                principal);

            // Gestione dell'eventuale URL richiesto prima del login
            if (!string.IsNullOrEmpty(model.ReturnUrl) &&
                Url.IsLocalUrl(model.ReturnUrl))
            {
                return LocalRedirect(model.ReturnUrl);
            }

            // Reindirizzamento in base al ruolo
            if (utente.IsAdmin)
                return RedirectToAction("Index", "Admin");
                
            return RedirectToAction("Index", "Home");
        }

        // Logout
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(
                CookieAuthenticationDefaults.AuthenticationScheme);

            return RedirectToAction("Index", "Login");
        }

        // Accesso negato
        [AllowAnonymous]
        [HttpGet]
        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}
