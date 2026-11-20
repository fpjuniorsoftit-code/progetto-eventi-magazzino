/*PasswordService.

Il suo compito sarà occuparsi esclusivamente di:
-Generare l'hash quando creiamo un utente.
- Verificare la password quando l'utente effettua il login.*/

using GestionEventiMagazzino.Models;
using Microsoft.AspNetCore.Identity;



namespace GestionEventiMagazzino.Services
{
    public class PasswordService
    {
        private readonly IPasswordHasher<User> passwordHasher; 

        public PasswordService(IPasswordHasher<User> passwordHasher)
        {
            this.passwordHasher = passwordHasher;
        }
        public string HashPassword(User user, string password) //questa è la formula che hasha la password
        {
            return passwordHasher.HashPassword(user, password); 
        }
        public bool VerifyPassword(User user, string password) /*Sono i due parametri in ingresso: l'oggetto dell'utente (che contiene il vecchio hash salvato nel database) e la password in chiaro che l'utente ha appena digitato nella schermata di login. */
        {
            var result = passwordHasher.VerifyHashedPassword(user, user.passwordHash, password);
            if (result == PasswordVerificationResult.Failed)
            {
                return false; //Password errata
            }
            else
            {
                return true; //password corretta
            }
        }
    }
}
