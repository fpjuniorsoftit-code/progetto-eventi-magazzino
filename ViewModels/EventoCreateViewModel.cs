using System.ComponentModel.DataAnnotations; 

namespace GestionEventiMagazzino.ViewModels
{
    public class EventoCreateViewModel
    {
        [Required(ErrorMessage = "Seleziona un assegnatario!")]
        [Display(Name = "Assegnatario")]
        public string Username { get; set; } = string.Empty;

        [Required(ErrorMessage = "Inserisci la data dell'evento!")]
        [Display(Name = "Data evento")]
        public DateOnly DataEvento { get; set; }

        [Required(ErrorMessage = "Inserisci l'ora di inizio.")]
        [Display(Name = "Ora inizio")]
        public TimeOnly OraInizio { get; set; }

        [Required(ErrorMessage = "Inserisci l'ora di fine.")]
        [Display(Name = "Ora fine")]
        public TimeOnly OraFine { get; set; }

        [Required(ErrorMessage = "Inserisci la descrizione dell'evento.")]
        [Display(Name = "Descrizione evento")]
        public string Evento { get; set; } = string.Empty;

        [Display(Name = "Operatori supplementari")]
        public string? AltriOperatori { get; set; }


        [MaxLength(255, ErrorMessage = "La nota non può superare i 255 caratteri!")]
        public string? Note { get; set; }

        [Required(ErrorMessage = "Seleziona uno stato.")]
        [Display(Name = "Stato")]
        public string Stato { get; set; } = string.Empty;

    }
}
