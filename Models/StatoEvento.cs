
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

    namespace GestionEventiMagazzino.Models
    {
        public class StatoEvento
        {
            [Key]
            [Column(TypeName = "varchar(20)")]
            
            public string statEvent { get; set; } // Questa è la Primary Key

            [Required]
            [Column(TypeName = "varchar(50)")]
            public string descrEvento { get; set; } // Es. Evento attivo, in sospeso, ecc.

            [Column(TypeName = "varchar(255)")]
            public string ?note { get; set; } //nullable
            

        }
    }

