using GestionEventiMagazzino.Data;
using GestionEventiMagazzino.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GestionEventiMagazzino.Controllers
{
    [Authorize] // Solo gli utenti autenticati possono accedere alla dashboard
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext context;

        public HomeController(ApplicationDbContext context)
        {
            this.context = context;
        }

        public async Task<IActionResult> Index(DateOnly? data, string? stato, string? username, int page = 1, int pageSize = 20)
        {
            DateOnly oggi = DateOnly.FromDateTime(DateTime.Today);
            DateOnly ieri = oggi.AddDays(-1);
            DateOnly limite = oggi.AddDays(5);

            // Dimensione pagina sicura (almeno 1 elemento)
            int dimensionePagina = Math.Max(1, pageSize);

            var query = context.Events
                .AsNoTracking()
                .Include(e => e.statoDettaglio)
                .Include(e => e.utente)
                .AsQueryable();

            // Senza filtri attivi mostra la finestra standard: ieri, oggi e prossimi 5 giorni (7 giorni totali)
            if (data == null && string.IsNullOrWhiteSpace(stato) && string.IsNullOrWhiteSpace(username))
            {
                query = query.Where(e => e.dataEvento >= ieri && e.dataEvento <= limite);
            }

            // Applica il filtro per data se impostato
            if (data != null)
            {
                query = query.Where(e => e.dataEvento == data.Value);
            }

            // Applica il filtro per stato se impostato
            if (!string.IsNullOrWhiteSpace(stato))
            {
                query = query.Where(e => e.stato == stato);
            }

            // Applica il filtro per operatore se impostato
            if (!string.IsNullOrWhiteSpace(username))
            {
                query = query.Where(e => e.username == username);
            }

            // Calcola il totale degli eventi filtrati e il numero complessivo di pagine (almeno 1)
            int totaleEventi = await query.CountAsync();
            int totalPages = Math.Max(1, (int)Math.Ceiling(totaleEventi / (double)dimensionePagina));
            int paginaCorrente = Math.Clamp(page, 1, totalPages);

            // Recupera solo gli eventi della pagina richiesta con ordinamento stabile
            var eventiPagina = await query
                .OrderBy(e => e.dataEvento)
                .ThenBy(e => e.oraInizio)
                .ThenBy(e => e.eventoID)
                .Skip((paginaCorrente - 1) * dimensionePagina)
                .Take(dimensionePagina)
                .ToListAsync();

            // Elenco username per il modale di filtro operatore
            ViewBag.Utenti = await context.Users
                .AsNoTracking()
                .OrderBy(u => u.username)
                .Select(u => u.username)
                .ToListAsync();

            // Mantiene i filtri attivi nella vista
            ViewBag.DataFiltro = data?.ToString("yyyy-MM-dd");
            ViewBag.StatoFiltro = stato;
            ViewBag.UsernameFiltro = username;

            // Passa i dati di paginazione a Index e _RigheEventi
            ViewBag.CurrentPage = paginaCorrente;
            ViewBag.TotalPages = totalPages;

            // Se la richiesta arriva via fetch/AJAX, restituisce solo le righe parziali
            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return PartialView("_RigheEventi", eventiPagina);
            }

            // Altrimenti restituisce la pagina completa della dashboard
            return View(eventiPagina);
        }
    }
}