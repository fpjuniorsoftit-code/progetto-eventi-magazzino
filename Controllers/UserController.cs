using GestionEventiMagazzino.Services;
using GestionEventiMagazzino.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace GestionEventiMagazzino.Controllers
{
    [Authorize(Roles = "Admin")]
    public class UsersController : Controller
    {
        private readonly UserService userService;

        public UsersController(UserService userService)
        {
            this.userService = userService;
        }

        // Visualizzazione modulo
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }



        //ricerca utenti per il pannello amministratore(collabora con user service)
        [HttpGet]
        public async Task<IActionResult> Search(string term)
        {
            var usernames = await userService.SearchUsernamesAsync(term ?? ""); //richiamo il metodo in userservice

            return Json(usernames);
        }

        // Creazione utente
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateUserViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            try
            {
                await userService.CreateUserAsync(
                    username: model.Username,
                    password: model.Password,
                    email: model.Email,
                    isAdmin: model.IsAdmin,
                    note: model.Note
                );



                return RedirectToAction(nameof(Create));
            }
            catch (ArgumentException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
            }

            return View(model);
        }

        // Visualizzazione modulo
        [HttpGet]
        public async Task<IActionResult> Update(string username)
        {
            // Recupero l'utente dal database
            var user = await userService.GetUserByUsernameAsync(username);

            if (user == null)
                return NotFound();

            // Carico i dati nel ViewModel
            var model = new UpdateUserViewModel
            {
                Username = user.username,
                Email = user.email,
                Note = user.note,
                IsAdmin = user.IsAdmin
            };

            return View(model);
        }


        // aggiornamento utente
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(UpdateUserViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            try
            {
                await userService.UpdateUserAsync(
                    username: model.Username,
                    password: model.Password,
                    email: model.Email,
                    isAdmin: model.IsAdmin,
                    note: model.Note
                );



                return RedirectToAction("Index", "Admin"); // il redirectToAction appena premuto il bottone aggiorna mi rimanda nel pannello admin
            }
            catch (ArgumentException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
            }

            return View(model);
        }
        // eliminazione utente
        [HttpPost]
        [ValidateAntiForgeryToken] //consente di proteggere il form da attacchi. 
        public async Task<IActionResult> Delete(string username) //Task è tipo una promise in javascript. IActionResult è il tipo di ritorno e indica che il metodo farà qualcosa - un'azione
        {
            // Impedisco all'Admin di disattivare il proprio account
            if (User.Identity?.Name == username)
            {
                TempData["Errore"] = "Non puoi disattivare il tuo stesso account.";
                return RedirectToAction("Index", "Admin");
            }

            bool risultato = await userService.DeleteUserAsync(username); //await serve ad attendere la risposta dal database. bool è usato per indicare se l'operazione è andata a buon fine o no.
             if (!risultato)                                                            
            {
                TempData["Errore"] = "Utente non trovato oppure già disattivato.";
                return RedirectToAction("Index","Admin");
            }
            TempData["Successo"] = "Utente disattivato correttamente.";
            return RedirectToAction("Index" ,"Admin"); //dopo la cancellazione riporta al pannello admin
        }
    } 
}

