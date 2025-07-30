using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;
using ProyectoDistri2.DAL;
using ProyectoDistri2.Models;

namespace ProyectoDistri2.Negocio
{
    public class ReservaBN
    {
        private readonly GestorReserva db = new GestorReserva();

        public IEnumerable<Reserva> ConsultarPorDia(DateTime fecha) =>
            db.Reservas
              .Include(r => r.Usuario)
              .Include(r => r.Espacio)
              .Where(r => DbFunctions.TruncateTime(r.FechaInicio) == fecha.Date)
              .ToList();

        public IEnumerable<Reserva> ConsultarPorSemana(DateTime inicio)
        {
            var fin = inicio.AddDays(7);
            return db.Reservas
                     .Include(r => r.Usuario)
                     .Include(r => r.Espacio)
                     .Where(r => r.FechaInicio >= inicio && r.FechaFin < fin)
                     .ToList();
        }

        public IEnumerable<Reserva> ConsultarPorMes(int mes, int año) =>
            db.Reservas
              .Include(r => r.Usuario)
              .Include(r => r.Espacio)
              .Where(r => r.FechaInicio.Month == mes && r.FechaInicio.Year == año)
              .ToList();

        public IEnumerable<Reserva> FiltrarReservas(string usuario, string tipoEspacio, string estado)
        {
            var query = db.Reservas
                          .Include(r => r.Usuario)
                          .Include(r => r.Espacio)
                          .AsQueryable();

            if (!string.IsNullOrEmpty(usuario))
                query = query.Where(r => r.Usuario.Nombre.Contains(usuario));
            if (!string.IsNullOrEmpty(tipoEspacio))
                query = query.Where(r => r.Espacio.Tipo == tipoEspacio);
            if (!string.IsNullOrEmpty(estado))
                query = query.Where(r => r.Estado == estado);

            return query.ToList();
        }
    }
}
