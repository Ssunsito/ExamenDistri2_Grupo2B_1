// *****************************************************
// Examen2 – Controlador ReservasController
// Kenneth Andrés Pantoja Manobanda
// Santiago Pila
// Fecha: 30/07/2025
//
// RESULTADOS
// - Implementa endpoints para consultar y filtrar reservas (día, semana, mes).
// - Incluye autorización por roles para ciertas rutas.
//
// CONCLUSIONES
// - Centraliza la lógica de acceso a reservas vía Web API.
// - Facilita la integración de la capa de negocio con la capa de presentación.
// *****************************************************
using System;
using System.Web.Http;
using System.Data.Entity;
using ProyectoDistri2.Negocio;
using ClosedXML.Excel;
using System.IO;
using System.Net.Http;
using System.Net;
using DocumentFormat.OpenXml.Spreadsheet;
using ProyectoDistri2.DAL;
using System.Linq;

namespace ProyectoDistri2.WebAPI.Controllers
{
    
    [RoutePrefix("api/reservas")]
    public class ReservasController : ApiController
    {
        private readonly ReservaBN service = new ReservaBN();
        private readonly GestorReserva db = new GestorReserva();

        [HttpGet]
        [Route("dia")]
        public IHttpActionResult ConsultarPorDia(DateTime fecha) =>
            Ok(service.ConsultarPorDia(fecha));

        [HttpGet]
        [Route("semana")]
        public IHttpActionResult ConsultarPorSemana(DateTime inicio) =>
            Ok(service.ConsultarPorSemana(inicio));

        [HttpGet]
        [Route("mes")]
        public IHttpActionResult ConsultarPorMes(int mes, int año) =>
            Ok(service.ConsultarPorMes(mes, año));

        [HttpGet]
        [Authorize(Roles = "Admin,Coordinador")]
        [Route("filtrar")]
        public IHttpActionResult FiltrarReservas(string usuario = "", string tipoEspacio = "", string estado = "") =>
            Ok(service.FiltrarReservas(usuario, tipoEspacio, estado));

        [HttpGet]
        [Authorize(Roles = "Admin,Coordinador")]
        [Route("api/reservas/exportar/excel")]
        public HttpResponseMessage ExportarExcel()
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
                    ws.Cell(row, 2).Value = r.Usuario.Nombre;
                    ws.Cell(row, 3).Value = r.Espacio.Nombre;
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

    }
}
