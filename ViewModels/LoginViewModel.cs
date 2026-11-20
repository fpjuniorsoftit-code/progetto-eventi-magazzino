using System.ComponentModel.DataAnnotations;  /* Importa gli strumenti necessari per applicare le validazioni (le regole sui campi obbligatori o sui messaggi di errore).*/

namespace GestionEventiMagazzino.ViewModels
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Inserisci il nome utente.")]
        [Display(Name = "Nome utente")]
        public string Username { get; set; } = string.Empty; //empty evita di inizializzare la stringa come null, evitando problemi

        [Required(ErrorMessage = "Inserisci la password.")]
        [DataType(DataType.Password)] //questa parte dice ad asp.net che la tipologia di dato in questione è una password ,facendo in modo che i caratteri digitati dall'utente vengano nascosti dai classici pallini neri o asterischi.
        public string Password { get; set; } = string.Empty;

        public string? ReturnUrl { get; set; }
    }
}