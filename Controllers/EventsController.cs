using GestionEventiMagazzino.Data;
using GestionEventiMagazzino.Services;
using GestionEventiMagazzino.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace GestionEventiMagazzino.Controllers
{
    [Authorize(Roles = "Admin")]
    public class EventsController : Controller
    {
        private readonly ApplicationDbContext context;
        private readonly EventoService eventoService;

        private readonly IHubContext<EventiHub> hubContext; //con questa inietto signalR all'interno del controller
        public EventsController(
               ApplicationDbContext context,
               EventoService eventoService,
               IHubContext<EventiHub> hubContext)
        {
            this.context = context;
            this.eventoService = eventoService;
            this.hubContext = hubContext;
        }
        // Mostra il modulo di creazione
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            await CaricaListeAsync();

            return View();
        }
        // Riceve i dati del modulo
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(EventoCreateViewModel model)
        {
            if (!ModelState.IsValid)
            {
                await CaricaListeAsync(model.Username, model.Stato);
                return View(model);
            }

            try
            {
                await eventoService.CreateEventoAsync(
                    username: model.Username,
                    dataEvento: model.DataEvento,
                    oraInizio: model.OraInizio,
                    oraFine: model.OraFine,
                    evento: model.Evento,
                    stato: model.Stato,
                    note: model.Note,
                    altriOperatori: model.AltriOperatori
                );

                // Avviso tutte le dashboard collegate che gli eventi sono cambiati
                await hubContext.Clients.All.SendAsync("AggiornaEventi");

                return RedirectToAction("Index", "Home");
            }
            catch (ArgumentException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);

                await CaricaListeAsync(model.Username, model.Stato);

                return View(model);
            }
        }
                // Carica gli utenti e gli stati disponibili
        private async Task CaricaListeAsync(
            string? username = null,
            string? stato = null) 
        {
            var utenti = await context.Users
                .AsNoTracking()
                .Where(u => !u.isDeactivated) //QUESTO PERMETTE ALLA LISTA A TENDINA HTML DI NON FAR USCIRE GLI UTENTI DISATTIVATI
                .OrderBy(u => u.username)
                .ToListAsync();

            var stati = await context.StatiEventi
                .AsNoTracking()
                .OrderBy(s => s.descrEvento)
                .ToListAsync();

            ViewBag.Utenti = new SelectList( //viewbag è un oggetto dinamico. Non ha una struttura fissa. È comodissimo per cose marginali,sfrutta una caratteristica di C# chiamata dinamica (dynamic): puoi inventarti il nome della proprietà al volo
                utenti, "username", "username", username);

            ViewBag.Stati = new SelectList(
                stati, "statEvent", "descrEvento", stato);
        }





        // Mostra il modulo di modifica
        [HttpGet]
        public async Task<IActionResult> Update(int id)
        {
            // Cerco l'evento da modificare
            var evento = await context.Events
                .AsNoTracking()
                .FirstOrDefaultAsync(e => e.eventoID == id);

            if (evento == null)
                return NotFound();

            // Trasferisco i dati dell'evento nel ViewModel
            var model = new EventoCreateViewModel
            {
                Username = evento.username,
                DataEvento = evento.dataEvento,
                OraInizio = evento.oraInizio,
                OraFine = evento.oraFine,
                Evento = evento.evento,
                Stato = evento.stato,
                Note = evento.note,
                AltriOperatori = evento.altriOperatori
            };

            // Carico gli utenti e gli stati per i menu a tendina
            await CaricaListeAsync(model.Username, model.Stato);

            return View(model);
        }
        // Riceve i dati del modulo di modifica
        [HttpPost]
        [ValidateAntiForgeryToken]

        //scrivendo update nel metodo il browser nel percorso di ricerca si aspetterà Views/Events/Update 

        public async Task<IActionResult> Update(int id, EventoCreateViewModel model)
        {
            if (!ModelState.IsValid)
            {
                await CaricaListeAsync(model.Username, model.Stato);
                return View(model);
            }

            try
            {
                await eventoService.UpdateEventoAsync(
                    eventoID: id,
                    username: model.Username,
                    dataEvento: model.DataEvento,
                    oraInizio: model.OraInizio,
                    oraFine: model.OraFine,
                    evento: model.Evento,
                    stato: model.Stato,
                    note: model.Note,
                    altriOperatori: model.AltriOperatori
                );

                // Avviso tutte le dashboard collegate che gli eventi sono cambiati
                await hubContext.Clients.All.SendAsync("AggiornaEventi");
                return RedirectToAction("Index", "Home");
            }
            catch (ArgumentException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                await CaricaListeAsync(model.Username, model.Stato);
                return View("Update", model);
            }

        }
    }
}
