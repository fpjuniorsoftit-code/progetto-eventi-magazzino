using GestionEventiMagazzino.Models;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;

namespace GestionEventiMagazzino.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }
        public DbSet<User> Users { get; set; }
        public DbSet<EventoMagazzino> Events { get; set; }

        public DbSet<StatoEvento> StatiEventi { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder); //OnModelCreating: definisce le due relazioni tra le tabelle.

            //relazione User -> Eventi
            modelBuilder.Entity<EventoMagazzino>()
                .HasOne(e => e.utente)
                .WithMany()
                .HasForeignKey(e => e.username)
                .OnDelete(DeleteBehavior.Restrict); //Questo impedirà di eliminare un utente o uno stato ancora utilizzato da un evento.

            // Relazione StatoEvento -> Eventi
            modelBuilder.Entity<EventoMagazzino>()
                .HasOne(e => e.statoDettaglio)
                .WithMany()
                .HasForeignKey(e => e.stato)
                .OnDelete(DeleteBehavior.Restrict); //Questo impedirà di eliminare un utente o uno stato ancora utilizzato da un evento.

            // Inserimento degli stati iniziali
            modelBuilder.Entity<StatoEvento>().HasData(
                new StatoEvento
                {
                    statEvent = "IN CORSO",
                    descrEvento = "Evento in corso"
                },
                new StatoEvento
                {
                    statEvent = "SOSPESO",
                    descrEvento = "Evento sospeso"
                },
                new StatoEvento
                {
                    statEvent = "CANCELLATO",
                    descrEvento = "Evento cancellato"
                },
                new StatoEvento
                {
                    statEvent = "CHIUSO",
                    descrEvento = "Evento concluso"
                }
                    );
        }
    }
}

    