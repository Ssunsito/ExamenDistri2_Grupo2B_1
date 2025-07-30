// *****************************************************
// Examen– Clase GestorReserva
// Kenneth Andrés Pantoja Manobanda
// Santiago Pila
// Fecha: 30/07/2025
//
// RESULTADOS
// - Se creó el contexto de Entity Framework para la base de datos ReservaDB.
// - Incluye DbSet para Usuarios, Espacios y Reservas.
//
// CONCLUSIONES
// - Permite mapear las entidades del modelo hacia la base de datos.
// - Facilita la persistencia y las consultas con LINQ.
// *****************************************************
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
