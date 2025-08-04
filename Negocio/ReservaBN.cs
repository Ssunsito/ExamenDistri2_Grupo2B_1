// *****************************************************
// Proyecto 2 - Reserva.co
// Capa de Negocio - ReservaBN
// Kenneth Pantoja
// Santiago Pila
// Fecha: 03/08/2025
//
// RESULTADOS FINALES
// - Se implementaron consultas eficientes para diferentes periodos temporales
// - Se integró manejo de relaciones entre reservas, usuarios y espacios
// - Se implementó sistema de filtrado flexible y dinámico
// - Se optimizó el rendimiento con consultas LINQ
// - Se centralizó la lógica de negocio de reservas
//
// CONCLUSIONES
// 1. La implementación de consultas por periodos proporciona una gestión temporal eficiente
// 2. El manejo de relaciones mejora la integridad de los datos consultados
// 3. El sistema de filtrado permite búsquedas flexibles y precisas
// 4. La estructura de la capa de negocio facilita el mantenimiento y extensión
// 5. Las consultas optimizadas garantizan un rendimiento adecuado
// *************************************

using ProyectoDistri2.DAL;
using ProyectoDistri2.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

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
