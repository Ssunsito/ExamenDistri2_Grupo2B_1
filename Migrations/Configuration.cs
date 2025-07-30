namespace ProyectoDistri2.Migrations
{
    using ProyectoDistri2.Models;
    using System;
    using System.Collections.Generic;
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
            // 1. Usuarios
            var usuarios = new List<Usuario>
    {
        new Usuario { Nombre = "Kenneth", Rol = "Admin" },
        new Usuario { Nombre = "Maria", Rol = "Coordinador" },
        new Usuario { Nombre = "Pedro", Rol = "Profesor" },
        new Usuario { Nombre = "Luisa", Rol = "Profesor" },
        new Usuario { Nombre = "Jorge", Rol = "Coordinador" }
    };
            usuarios.ForEach(u => context.Usuarios.Add(u));
            context.SaveChanges();

            // 2. Espacios
            var espacios = new List<Espacio>
    {
        new Espacio { Nombre = "Aula 101", Tipo = "Aula" },
        new Espacio { Nombre = "Aula 102", Tipo = "Aula" },
        new Espacio { Nombre = "Lab Redes", Tipo = "Laboratorio" },
        new Espacio { Nombre = "Lab Software", Tipo = "Laboratorio" },
        new Espacio { Nombre = "Auditorio Principal", Tipo = "Auditorio" }
    };
            espacios.ForEach(e => context.Espacios.Add(e));
            context.SaveChanges();

            // 3. Reservas de prueba
            var reservas = new List<Reserva>
    {
        new Reserva
        {
            UsuarioId = usuarios[0].Id, // Kenneth (Admin)
            EspacioId = espacios[0].Id, // Aula 101
            FechaInicio = DateTime.Now.AddDays(-2),
            FechaFin = DateTime.Now.AddDays(-2).AddHours(2),
            Estado = "Aprobada"
        },
        new Reserva
        {
            UsuarioId = usuarios[1].Id, // Maria (Coordinador)
            EspacioId = espacios[2].Id, // Lab Redes
            FechaInicio = DateTime.Now,
            FechaFin = DateTime.Now.AddHours(3),
            Estado = "Pendiente"
        },
        new Reserva
        {
            UsuarioId = usuarios[2].Id, // Pedro (Profesor)
            EspacioId = espacios[4].Id, // Auditorio Principal
            FechaInicio = DateTime.Now.AddDays(1),
            FechaFin = DateTime.Now.AddDays(1).AddHours(4),
            Estado = "Pendiente"
        },
        new Reserva
        {
            UsuarioId = usuarios[3].Id, // Luisa (Profesor)
            EspacioId = espacios[1].Id, // Aula 102
            FechaInicio = DateTime.Now.AddDays(3),
            FechaFin = DateTime.Now.AddDays(3).AddHours(2),
            Estado = "Rechazada"
        },
        new Reserva
        {
            UsuarioId = usuarios[4].Id, // Jorge (Coordinador)
            EspacioId = espacios[3].Id, // Lab Software
            FechaInicio = DateTime.Now.AddDays(7),
            FechaFin = DateTime.Now.AddDays(7).AddHours(3),
            Estado = "Aprobada"
        }
    };
            reservas.ForEach(r => context.Reservas.Add(r));
            context.SaveChanges();
        }
    }
}
