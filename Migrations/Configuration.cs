namespace ProyectoDistri2.Migrations
{
    using ProyectoDistri2.Models;
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<ProyectoDistri2.DAL.GestorReserva>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(ProyectoDistri2.DAL.GestorReserva context)
        {
            var usuario = new Usuario { Nombre = "Kenneth", Rol = "Admin" };
            var espacio = new Espacio { Nombre = "Aula 101", Tipo = "Aula" };

            context.Usuarios.Add(usuario);
            context.Espacios.Add(espacio);
            context.Reservas.Add(new Reserva
            {
                Usuario = usuario,
                Espacio = espacio,
                FechaInicio = DateTime.Now,
                FechaFin = DateTime.Now.AddHours(2),
                Estado = "Aprobada"
            });

            context.SaveChanges();
        }
    }
}
