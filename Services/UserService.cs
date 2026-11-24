using GestionEventiMagazzino.Data;
using GestionEventiMagazzino.Models;
using Microsoft.EntityFrameworkCore;

namespace GestionEventiMagazzino.Services
{
    public class UserService
    {
        private readonly ApplicationDbContext context;
        private readonly PasswordService passwordService;

        public UserService(
            ApplicationDbContext context,
            PasswordService passwordService)
        {
            this.context = context;
            this.passwordService = passwordService; // Iniezione del servizio di hashing delle password
        }

        //recupera gli username da modificare o eliminare
        public async Task<List<string>> GetUsernamesAsync()
        {
            return await context.Users
                .AsNoTracking()
                .Where(u => !u.isDeactivated) 
                .OrderBy(u => u.username)
                .Select(u => u.username)
                .ToListAsync();
        }

        //Cerca gli utenti attivi per username(serve in pannello admin per avere barra di ricerca anzichè lista completa utenti)
        public async Task<List<string>> SearchUsernamesAsync(string ricerca)
        {
            ricerca = ricerca.Trim(); //elimina spazi vuoti

            if (ricerca.Length < 2)
                return new List<string>();

            return await context.Users
                  .AsNoTracking()
                    .Where(u => !u.isDeactivated &&
                                u.username.Contains(ricerca)) //esclude utenti disattivati
                    .OrderBy(u => u.username)
                    .Select(u => u.username)
                    .Take(10)//restituisci massimo 10 risultati
                    .ToListAsync();

        }



        // Recupera un utente(ed i suoi dati) tramite username
        public async Task<User?> GetUserByUsernameAsync(string username)
        {
            return await context.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.username == username);
        }
        public async Task<User> CreateUserAsync(
            string username,
            string password,
            string email,
            string note,
            bool isAdmin = false)
        {
            // Controllo dei dati
            username = username.Trim();
            email = email.Trim();

            if (string.IsNullOrWhiteSpace(username))
                throw new ArgumentException("Username obbligatorio.");

            if (string.IsNullOrWhiteSpace(password) || password.Length < 8)
                throw new ArgumentException("La password deve contenere almeno 8 caratteri.");

            if (string.IsNullOrWhiteSpace(email))
                throw new ArgumentException("Email obbligatoria.");

            if (note.Length > 255)
                throw new ArgumentException("La nota non può superare i 255 caratteri.");

            // Controllo username già esistente
            bool esiste = await context.Users
                .AnyAsync(u => u.username == username);

            if (esiste)
                throw new InvalidOperationException("Username già esistente.");

            // Creazione utente
            var user = new User
            {
                username = username,
                email = email,
                IsAdmin = isAdmin,
                note = note
            };

            // Generazione hash password
            user.passwordHash = passwordService.HashPassword(
                user,
                password
            );

            // Salvataggio database
            context.Users.Add(user);

            await context.SaveChangesAsync();

            return user;
        }

        //aggiornamento user

        public async Task UpdateUserAsync(
            string username,
            string? password,
            string email,
            string? note,
            bool isAdmin = false)
        {
            // Verifico che lo user esista
            var userEsistente = await context.Users
                .FirstOrDefaultAsync(u => u.username == username);

            if (userEsistente == null)
                throw new ArgumentException("User non trovato.");

            // Verifico che l'email sia unica
            bool emailEsistente = await context.Users
                .AnyAsync(u => u.email == email && u.username != username);

            if (emailEsistente)
                throw new ArgumentException("L'email è già in uso.");

            // Aggiorno i dati dell'utente
            userEsistente.email = email;
            userEsistente.IsAdmin = isAdmin;
            userEsistente.note = note;

            // Aggiorno la password solo se ne è stata inserita una nuova
            if (!string.IsNullOrWhiteSpace(password))
            {
                userEsistente.passwordHash =
                    passwordService.HashPassword(userEsistente, password);
            }

            await context.SaveChangesAsync();
        }
        //disattivazione user
        public async Task<bool> DeleteUserAsync(
            string username
            )
        {
            // Verifico che lo user esista
            var userEsistente = await context.Users
                .FirstOrDefaultAsync(u => u.username == username);

            if (userEsistente == null || userEsistente.isDeactivated)
                return false;

            userEsistente.isDeactivated = true;
            await context.SaveChangesAsync(); //salvo le modifiche sul db

            return true;


        }
    }
}