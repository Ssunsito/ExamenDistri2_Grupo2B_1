// *****************************************************
// Proyecto 2 - Reserva.co
// Kenneth Pantoja
// Santiago Pila
// Fecha: 03/08/2025
//
// RESULTADOS
// - Se creó el contexto de Entity Framework para la base de datos ReservaDB.
// - Incluye DbSet para Usuarios, Espacios y Reservas.
//
// CONCLUSIONES
// - Permite mapear las entidades del modelo hacia la base de datos.
// - Facilita la persistencia y las consultas con LINQ.
// *****************************************************
using ProyectoDistri2.Models;
using System.Data.Entity;

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
