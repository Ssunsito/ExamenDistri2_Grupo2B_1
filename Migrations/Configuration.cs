// *****************************************************
// Proyecto 2 - Reserva.co
// Configuración de Migraciones
// Kenneth Pantoja
// Santiago Pila
// Fecha: 03/08/2025
//
// RESULTADOS FINALES
// - Se implementó configuración de migraciones con Entity Framework Core
// - Se estableció proceso de inicialización de datos para desarrollo
// - Se configuró limpieza y reseeding de tablas
// - Se implementó estructura de datos para usuarios, espacios y reservas
// - Se establecieron relaciones entre entidades
//
// CONCLUSIONES
// 1. La configuración de migraciones permite un control preciso sobre la evolución del esquema
// 2. El proceso de seeding facilita la inicialización del sistema en diferentes entornos
// 3. La estructura de datos implementada soporta las relaciones necesarias entre usuarios, espacios y reservas
// 4. El manejo de estados de reserva permite un control efectivo del sistema
// 5. La organización del código facilita su mantenimiento y extensión
// *************************************

namespace ProyectoDistri2.Migrations
{
    using ProyectoDistri2.Models;
    using System;
    using System.Collections.Generic;
    using System.Data.Entity.Migrations;

    internal sealed class Configuration : DbMigrationsConfiguration<ProyectoDistri2.DAL.GestorReserva>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(ProyectoDistri2.DAL.GestorReserva context)
        {
            // Limpieza de tablas (solo para desarrollo)
            context.Reservas.RemoveRange(context.Reservas);
            context.Usuarios.RemoveRange(context.Usuarios);
            context.Espacios.RemoveRange(context.Espacios);
            context.SaveChanges();

            // 1️⃣ Usuarios
            var usuarios = new List<Usuario>
    {
        new Usuario { Nombre = "Kenneth", Rol = "Admin", Password = "1234" },
        new Usuario { Nombre = "Maria", Rol = "Coordinador", Password = "abcd" },
        new Usuario { Nombre = "Pedro", Rol = "Profesor", Password = "pass1" },
        new Usuario { Nombre = "Luisa", Rol = "Profesor", Password = "pass2" },
        new Usuario { Nombre = "Jorge", Rol = "Coordinador", Password = "admin2" }
    };
            context.Usuarios.AddRange(usuarios);
            context.SaveChanges();

            // 2️⃣ Espacios
            var espacios = new List<Espacio>
    {
        new Espacio { Nombre = "Aula 101", Tipo = "Aula" },
        new Espacio { Nombre = "Aula 102", Tipo = "Aula" },
        new Espacio { Nombre = "Lab Redes", Tipo = "Laboratorio" },
        new Espacio { Nombre = "Lab Software", Tipo = "Laboratorio" },
        new Espacio { Nombre = "Auditorio Principal", Tipo = "Auditorio" }
    };
            context.Espacios.AddRange(espacios);
            context.SaveChanges();

            // 3️⃣ Reservas
            var reservas = new List<Reserva>
    {
        new Reserva
        {
            UsuarioId = usuarios[0].Id,
            EspacioId = espacios[0].Id,
            FechaInicio = DateTime.Now.AddDays(-2),
            FechaFin = DateTime.Now.AddDays(-2).AddHours(2),
            Estado = "Aprobada"
        },
        new Reserva
        {
            UsuarioId = usuarios[1].Id,
            EspacioId = espacios[2].Id,
            FechaInicio = DateTime.Now,
            FechaFin = DateTime.Now.AddHours(3),
            Estado = "Pendiente"
        },
        new Reserva
        {
            UsuarioId = usuarios[2].Id,
            EspacioId = espacios[4].Id,
            FechaInicio = DateTime.Now.AddDays(1),
            FechaFin = DateTime.Now.AddDays(1).AddHours(4),
            Estado = "Pendiente"
        },
        new Reserva
        {
            UsuarioId = usuarios[3].Id,
            EspacioId = espacios[1].Id,
            FechaInicio = DateTime.Now.AddDays(3),
            FechaFin = DateTime.Now.AddDays(3).AddHours(2),
            Estado = "Rechazada"
        },
        new Reserva
        {
            UsuarioId = usuarios[4].Id,
            EspacioId = espacios[3].Id,
            FechaInicio = DateTime.Now.AddDays(7),
            FechaFin = DateTime.Now.AddDays(7).AddHours(3),
            Estado = "Aprobada"
        }
    };
            context.Reservas.AddRange(reservas);
            context.SaveChanges();
        }
    }
}
