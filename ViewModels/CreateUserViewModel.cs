using System.ComponentModel.DataAnnotations; /* Importa gli strumenti necessari per applicare le validazioni (le regole sui campi obbligatori o sui messaggi di errore).*/

namespace GestionEventiMagazzino.ViewModels
{
    public class CreateUserViewModel
    {
        [Required(ErrorMessage = "Il nome utente è obbligatorio!")]
        [MaxLength(50)]
        public string Username { get; set; } = string.Empty; //empty evita di inizializzare la stringa come null, evitando problemi

        [Required(ErrorMessage = "La password è obbligatoria!")]
        [MinLength(8, ErrorMessage = "La password deve essere lunga almeno 8 caratteri!")]
        [MaxLength(500)]

        public string Password { get; set; } = string.Empty;

        [Required(ErrorMessage = "L'email è obbligatoria!")]
        [EmailAddress(ErrorMessage = "L'email inserita non è valida!")]
        [MaxLength(150)]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Conferma la password!")]
        [Compare(nameof(Password), ErrorMessage = "Le password non coincidono!")]
        [DataType(DataType.Password)] //I caratteri digitati verranno nascosti dai classici pallini neri o asterischi
        public string ConfirmPassword { get; set; } = string.Empty;


        [MaxLength(255, ErrorMessage = "La nota non può superare i 255 caratteri!")]
        public string? Note { get; set; }

        public bool IsAdmin { get; set; } = false;
    }
}
