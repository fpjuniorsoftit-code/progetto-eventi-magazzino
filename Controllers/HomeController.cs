using GestionEventiMagazzino.Data;
using GestionEventiMagazzino.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Diagnostics;

namespace GestionEventiMagazzino.Controllers
{
    [Authorize] //solo gli utenti utenticati vedranno la home page
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext context;

        public HomeController(ApplicationDbContext context)
        {
            this.context = context;
        }
        public async Task<IActionResult> Index(DateOnly? data, string? stato, string username)
        {
            DateOnly oggi = DateOnly.FromDateTime(DateTime.Today);
            DateOnly ieri = oggi.AddDays(-1); //ADD days permette di mettere o togliere giorni alla data odierna.
            DateOnly limite = oggi.AddDays(5);


            var query = context.Events
                .AsNoTracking()
                .Include(e => e.statoDettaglio)
                .Include(e => e.utente)
                .AsQueryable();

            // Se non è stata scelta una data e non c'è uno stato,
        
            if (data == null && string.IsNullOrWhiteSpace(stato)
                && string.IsNullOrWhiteSpace(username)) //questa if dice che se non c'è filtro stato o operatore mette la dashboard dei prossimi 5 giorni con ieri e oggi anche quindi 7gg totali.
            {
                query = query.Where(e => e.dataEvento >= ieri && e.dataEvento <= limite);
            }

            // Se è stata scelta una data, applico il filtro.
            if (data != null)
            {
                query = query.Where(e => e.dataEvento == data.Value);
            }

            // Se è stato scelto uno stato, applico anche questo filtro.
            if (!string.IsNullOrWhiteSpace(stato))
            {
                query = query.Where(e => e.stato == stato);
            }
            //se è stato scelto uno username applico anche questo filtro
            if (!string.IsNullOrWhiteSpace(username))
            {
                query = query.Where(e => e.username == username);
            }

            //consente di visualizzare gli username disponibili per il filtro nella view
            ViewBag.Utenti = await context.Users
            .AsNoTracking()
            .OrderBy(u => u.username)
            .Select(u => u.username)
            .ToListAsync();

            var eventi = await query
                .OrderBy(e => e.oraInizio)
                .ToListAsync();

            // Memorizzo i filtri attivi per comunicarli alla View.
            ViewBag.DataFiltro = data?.ToString("yyyy-MM-dd");//la viewbag conterrà la data selezionata
            ViewBag.StatoFiltro = stato; //la viewbag conterrà lo stato selezionato
            ViewBag.UsernameFiltro = username; //la viewbag serve per passare dati dal controller alla view. in questo caso serve a tenere i filtri attivi caricati dopo la ricerca.

            //se la richiesta arriva da javascript restituisco solo le righe degli eventi
            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return PartialView("_RigheEventi", eventi);
            }

            // Altrimenti restituisco la dashboard completa
            return View(eventi);
        }
    }
}
