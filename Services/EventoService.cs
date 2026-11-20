using GestionEventiMagazzino.Data;
using GestionEventiMagazzino.Models;
using Microsoft.EntityFrameworkCore;



namespace GestionEventiMagazzino.Services
{
    public class EventoService
    {
        private readonly ApplicationDbContext context;

        public EventoService(ApplicationDbContext context)
        {
            this.context = context;
        }
        public async Task   CreateEventoAsync(
            string username,
            DateOnly dataEvento,
            TimeOnly oraInizio,
            TimeOnly oraFine,
            string evento,
            string note,
            string altriOperatori,
            string stato)
        {
            //verifica che l'assegnatario esiste e sia attivo
            bool utenteEsiste = await context.Users.AnyAsync(u => u.username == username && !u.isDeactivated);
            if (!utenteEsiste) 

                throw new ArgumentException($"L'utente con username '{username}' non esiste oppure è disattivato.");

            //verifica orario corretto
            if (oraFine <= oraInizio)
                throw new ArgumentException(
                    "L'ora di fine deve essere successiva all'ora di inizio.");

            // Creazione nuovo evento
            var nuovoEvento = new EventoMagazzino
            {
                username = username,
                timeStamp = DateTime.Now,
                dataEvento = dataEvento,
                oraInizio = oraInizio,
                oraFine = oraFine,
                evento = evento,
                stato = stato,
                note = note,
                altriOperatori = altriOperatori
            };

            // Salviamo nel database
            context.Events.Add(nuovoEvento);

            await context.SaveChangesAsync();
        }

            //aggiornamento evento

public async Task UpdateEventoAsync(
    int eventoID,
    string username,
    DateOnly dataEvento,
    TimeOnly oraInizio,
    TimeOnly oraFine,
    string evento,
    string stato,
    string note,
    string? altriOperatori)
        {
            // Verifico che l'evento esista
            var eventoEsistente = await context.Events
                .FirstOrDefaultAsync(e => e.eventoID == eventoID);

            if (eventoEsistente == null)
                throw new ArgumentException("Evento non trovato.");

           
            //verifica che l'assegnatario esiste e sia attivo(QUESTO PROTEGGE SALVATAGGIO LATO SERVER: NON PERMETTE SALVATAGGIO NEL DB SE UTENTE E' DISATTIVATO O NON ESISTE)
            bool utenteEsiste = await context.Users.AnyAsync(u => u.username == username && !u.isDeactivated);
            if (!utenteEsiste)

                throw new ArgumentException($"L'utente con username '{username}' non esiste oppure è disattivato.");

            // Verifico che lo stato esista
            bool statoEsistente = await context.StatiEventi
                .AnyAsync(s => s.statEvent == stato);

            if (!statoEsistente)
                throw new ArgumentException("Lo stato selezionato non esiste.");

            // Verifico la correttezza degli orari
            if (oraFine <= oraInizio)
                throw new ArgumentException("L'ora di fine deve essere successiva all'ora di inizio.");

            // Aggiorno i dati dell'evento
            eventoEsistente.username = username;
            eventoEsistente.dataEvento = dataEvento;
            eventoEsistente.oraInizio = oraInizio;
            eventoEsistente.oraFine = oraFine;
            eventoEsistente.evento = evento;
            eventoEsistente.stato = stato;
            eventoEsistente.note = note;
            eventoEsistente.altriOperatori = altriOperatori;

            await context.SaveChangesAsync(); //salvataggio nel db
        }

    }
}
        

