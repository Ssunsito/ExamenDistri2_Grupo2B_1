using System.Data.Entity;
using ProyectoDistri2.Models;

namespace ProyectoDistri2.DAL
{
    public class GestorReserva : DbContext
    {
        public GestorReserva() : base("ReservaDB") { }

        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Espacio> Espacios { get; set; }
        public DbSet<Reserva> Reservas { get; set; }
    }
}
