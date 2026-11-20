using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GestionEventiMagazzino.Models
{
    public class EventoMagazzino
    {
        [Key]
        public int eventoID { get; set; }

        [Required]
        [MaxLength(50)]
        public string username { get; set; }

        [ForeignKey("username")]
        public User utente { get; set; }

        [Required]
        public DateTime timeStamp { get; set; }

        [Required]
        public DateOnly dataEvento { get; set; }

        public TimeOnly oraInizio { get; set; }

        public TimeOnly oraFine { get; set; }

        [Required]
        [Column(TypeName = "nvarchar(max)")]
        public string evento { get; set; }

        [Required]
        [Column(TypeName = "varchar(20)")]
        public string stato { get; set; }

        [Column(TypeName = "nvarchar(255)")]
        public string? altriOperatori { get; set; }

        [MaxLength(255)]
        public string? note { get; set; } //nullable

        [ForeignKey("stato")]
        public StatoEvento statoDettaglio { get; set; }

    }
}