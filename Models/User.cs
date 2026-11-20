using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GestionEventiMagazzino.Models
{
    public class User
    {
        [Key]
        [Required]
        [MaxLength(50)]
        public string username { get; set; }

        [Required]
        [Column(TypeName = "varchar(500)")]

        public string passwordHash { get; set; }

        [Required]
        [Column(TypeName = "varchar(150)")]
        public string email { get; set; }

        [MaxLength(255)]
        public string? note { get; set; } //nullable

        public bool isDeactivated { get; set; } = false; //un utente di default è attivo.
        public bool IsAdmin { get; set; } = false; //un nuovo utente non diventa amministratore per errore.
    }
}
