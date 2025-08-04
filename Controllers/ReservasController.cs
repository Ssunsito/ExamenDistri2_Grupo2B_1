// *****************************************************
// Proyecto 2 – Reserva.co
// Controlador ReservasController
// Kenneth Pantoja
// Santiago Pila
// Fecha: 03/08/2025
//
// RESULTADOS FINALES
// - Se implementaron endpoints REST para consultar, filtrar, crear, editar, aprobar, rechazar y eliminar reservas.
// - Se integró autorización basada en roles para proteger operaciones críticas y acceso a historial.
// - Se centralizó el manejo de excepciones y la validación de conflictos de reservas para robustez y consistencia.
//
// CONCLUSIONES
// 1. La centralización de la lógica de reservas en el controlador facilita el mantenimiento y la escalabilidad.
// 2. El uso de SafeExecute mejora la robustez y la experiencia del usuario al manejar errores de forma uniforme.
// 3. La validación de conflictos y la autorización por roles aseguran integridad y seguridad en la gestión de reservas.
// *****************************************************

using ClosedXML.Excel;
using ProyectoDistri2.DAL;
using ProyectoDistri2.Models;
using ProyectoDistri2.Negocio;
using System;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace ProyectoDistri2.WebAPI.Controllers
{
    [RoutePrefix("api/reservas")]
    public class ReservasController : ApiController
    {
        private readonly ReservaBN service = new ReservaBN();
        private readonly GestorReserva db = new GestorReserva();

        // Método auxiliar para manejo de excepciones
        private IHttpActionResult SafeExecute(Func<IHttpActionResult> action)
        {
            try
            {
                return action();
            }
            catch (System.Data.SqlClient.SqlException ex)
            {
                return Content(HttpStatusCode.BadRequest, new
                {
                    error = "Error en la base de datos. Verifique IDs y relaciones.",
                    detalle = ex.Message
                });
            }
            catch (Exception ex)
            {
                return Content(HttpStatusCode.InternalServerError, new
                {
                    error = "Error inesperado en el servidor.",
                    detalle = ex.Message
                });
            }
        }

        // -------------------- CONSULTAS --------------------

        [HttpGet]
        [Route("dia")]
        public IHttpActionResult ConsultarPorDia(DateTime fecha) =>
            SafeExecute(() => Ok(service.ConsultarPorDia(fecha)));

        [HttpGet]
        [Route("semana")]
        public IHttpActionResult ConsultarPorSemana(DateTime inicio) =>
            SafeExecute(() => Ok(service.ConsultarPorSemana(inicio)));

        [HttpGet]
        [Route("mes")]
        public IHttpActionResult ConsultarPorMes(int mes, int año) =>
            SafeExecute(() => Ok(service.ConsultarPorMes(mes, año)));

        [HttpGet]
        [Authorize(Roles = "Admin,Coordinador")]
        [Route("filtrar")]
        public IHttpActionResult FiltrarReservas(string usuario = "", string tipoEspacio = "", string estado = "") =>
            SafeExecute(() => Ok(service.FiltrarReservas(usuario, tipoEspacio, estado)));

        [HttpGet]
        [Authorize(Roles = "Admin,Coordinador")]
        [Route("")]
        public IHttpActionResult GetReservas() =>
            SafeExecute(() => Ok(db.Reservas
                .Include(r => r.Usuario)
                .Include(r => r.Espacio)
                .ToList()));

        // Obtener reserva por ID (para modales)
        [HttpGet]
        [Authorize(Roles = "Admin,Coordinador")]
        [Route("{id:int}")]
        public IHttpActionResult GetReservaPorId(int id) =>
            SafeExecute(() =>
            {
                var reserva = db.Reservas
                    .Include(r => r.Usuario)
                    .Include(r => r.Espacio)
                    .FirstOrDefault(r => r.Id == id);
                if (reserva == null) return NotFound();
                return Ok(reserva);
            });

        // -------------------- EXPORTACIÓN --------------------

        [HttpGet]
        [Authorize(Roles = "Admin,Coordinador")]
        [Route("exportar/excel")]
        public HttpResponseMessage ExportarExcel()
        {
            try
            {
                var reservas = db.Reservas.Include(r => r.Usuario).Include(r => r.Espacio).ToList();
                using (var workbook = new XLWorkbook())
                {
                    var ws = workbook.Worksheets.Add("Reservas");
                    ws.Cell(1, 1).Value = "ID";
                    ws.Cell(1, 2).Value = "Usuario";
                    ws.Cell(1, 3).Value = "Espacio";
                    ws.Cell(1, 4).Value = "Fecha Inicio";
                    ws.Cell(1, 5).Value = "Fecha Fin";

                    int row = 2;
                    foreach (var r in reservas)
                    {
                        ws.Cell(row, 1).Value = r.Id;
                        ws.Cell(row, 2).Value = r.Usuario?.Nombre;
                        ws.Cell(row, 3).Value = r.Espacio?.Nombre;
                        ws.Cell(row, 4).Value = r.FechaInicio;
                        ws.Cell(row, 5).Value = r.FechaFin;
                        row++;
                    }

                    using (var stream = new MemoryStream())
                    {
                        workbook.SaveAs(stream);
                        var result = new HttpResponseMessage(HttpStatusCode.OK)
                        {
                            Content = new ByteArrayContent(stream.ToArray())
                        };
                        result.Content.Headers.ContentType =
                            new System.Net.Http.Headers.MediaTypeHeaderValue("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
                        result.Content.Headers.ContentDisposition =
                            new System.Net.Http.Headers.ContentDispositionHeaderValue("attachment")
                            {
                                FileName = "Reservas.xlsx"
                            };
                        return result;
                    }
                }
            }
            catch (Exception ex)
            {
                var response = new HttpResponseMessage(HttpStatusCode.InternalServerError);
                response.Content = new StringContent("Error al exportar: " + ex.Message);
                return response;
            }
        }

        // -------------------- CRUD DE RESERVAS --------------------

        [HttpPost]
        [Route("")]
        public IHttpActionResult CrearReserva(Reserva reserva) =>
             SafeExecute(() =>
             {
                 if (!ModelState.IsValid)
                     return BadRequest(ModelState);

                 // 🔹 Validar conflicto de horario
                 bool existeConflicto = db.Reservas.Any(r =>
                     r.EspacioId == reserva.EspacioId &&
                     r.Estado == "Aprobada" &&
                     r.FechaInicio < reserva.FechaFin &&
                     r.FechaFin > reserva.FechaInicio
                 );

                 if (existeConflicto)
                     return Content(HttpStatusCode.BadRequest, new
                     {
                         error = "Conflicto de reserva",
                         detalle = "Ya existe una reserva aprobada que se superpone con este horario en el mismo espacio."
                     });

                 reserva.Estado = "Pendiente";
                 db.Reservas.Add(reserva);
                 db.SaveChanges();
                 return Ok(reserva);
             });

        [HttpPut]
        [Authorize(Roles = "Admin,Coordinador")]
        [Route("{id:int}")]
        public IHttpActionResult EditarReserva(int id, Reserva reserva) =>
            SafeExecute(() =>
            {
                if (!ModelState.IsValid || id != reserva.Id)
                    return BadRequest(ModelState);

                var reservaExistente = db.Reservas.Find(id);
                if (reservaExistente == null)
                    return NotFound();

                // 🔹 Validar conflicto
                bool existeConflicto = db.Reservas.Any(r =>
                    r.Id != reserva.Id &&
                    r.EspacioId == reserva.EspacioId &&
                    r.Estado == "Aprobada" &&
                    r.FechaInicio < reserva.FechaFin &&
                    r.FechaFin > reserva.FechaInicio
                );

                if (existeConflicto)
                    return Content(HttpStatusCode.BadRequest, new
                    {
                        error = "Conflicto de reserva",
                        detalle = "No se puede guardar la reserva: existe otra reserva aprobada en el mismo horario y espacio."
                    });

                // Actualizar datos
                reservaExistente.EspacioId = reserva.EspacioId;
                reservaExistente.FechaInicio = reserva.FechaInicio;
                reservaExistente.FechaFin = reserva.FechaFin;
                reservaExistente.Estado = reserva.Estado;
                db.SaveChanges();

                return Ok(reservaExistente);
            });

        [HttpPut]
        [Authorize(Roles = "Admin,Coordinador")]
        [Route("aprobar/{id:int}")]
        public IHttpActionResult AprobarReserva(int id) =>
            SafeExecute(() =>
            {
                var reserva = db.Reservas.Find(id);
                if (reserva == null) return NotFound();

                bool existeConflicto = db.Reservas.Any(r =>
                    r.Id != reserva.Id &&
                    r.EspacioId == reserva.EspacioId &&
                    r.Estado == "Aprobada" &&
                    r.FechaInicio < reserva.FechaFin &&
                    r.FechaFin > reserva.FechaInicio
                );

                if (existeConflicto)
                    return Content(HttpStatusCode.BadRequest, new
                    {
                        error = "Conflicto de reserva",
                        detalle = "No se puede aprobar: existe otra reserva aprobada en el mismo horario y espacio."
                    });

                reserva.Estado = "Aprobada";
                db.SaveChanges();
                return Ok(reserva);
            });

        [HttpPut]
        [Authorize(Roles = "Admin,Coordinador")]
        [Route("rechazar/{id:int}")]
        public IHttpActionResult RechazarReserva(int id) =>
            SafeExecute(() =>
            {
                var reserva = db.Reservas.Find(id);
                if (reserva == null) return NotFound();
                reserva.Estado = "Rechazada";
                db.SaveChanges();
                return Ok(reserva);
            });

        [HttpDelete]
        [Authorize(Roles = "Admin,Coordinador")]
        [Route("{id:int}")]
        public IHttpActionResult BorrarReserva(int id) =>
            SafeExecute(() =>
            {
                var reserva = db.Reservas.Find(id);
                if (reserva == null) return NotFound();
                db.Reservas.Remove(reserva);
                db.SaveChanges();
                return Ok(reserva);
            });

        // -------------------- HISTORIAL --------------------

        [HttpGet]
        [Authorize]
        [Route("historial/usuario/{idUsuario:int}")]
        public IHttpActionResult HistorialPorUsuario(int idUsuario) =>
            SafeExecute(() =>
            {
                // Si es profesor, solo puede ver su propio historial
                if (User.IsInRole("Profesor"))
                {
                    string nombreUsuario = User.Identity.Name;
                    var usuarioActual = db.Usuarios.FirstOrDefault(u => u.Nombre == nombreUsuario);
                    if (usuarioActual == null || usuarioActual.Id != idUsuario)
                    {
                        return Content(HttpStatusCode.Forbidden, new
                        {
                            error = "Acceso denegado",
                            detalle = "Un profesor solo puede consultar su propio historial."
                        });
                    }
                }

                var historial = db.Reservas
                    .Include(r => r.Usuario)
                    .Include(r => r.Espacio)
                    .Where(r => r.UsuarioId == idUsuario)
                    .OrderByDescending(r => r.FechaInicio)
                    .ToList();

                return Ok(historial);
            });

        [HttpGet]
        [Authorize(Roles = "Admin,Coordinador")]
        [Route("historial/espacio/{idEspacio:int}")]
        public IHttpActionResult HistorialPorEspacio(int idEspacio) =>
            SafeExecute(() =>
            {
                var historial = db.Reservas
                    .Include(r => r.Usuario)
                    .Include(r => r.Espacio)
                    .Where(r => r.EspacioId == idEspacio)
                    .OrderByDescending(r => r.FechaInicio)
                    .ToList();

                return Ok(historial);
            });
    }
}
