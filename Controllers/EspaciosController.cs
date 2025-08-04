// *****************************************************
// Proyecto Distri2 - Sistema de Gestión de Espacios
// Controlador EspaciosController
// Kenneth Pantoja 
// Santiago Pila
// Fecha: 03/08/2025
//
// RESULTADOS FINALES
// - Se implementaron endpoints REST completos para gestión de espacios físicos.
// - Se centralizó el manejo de excepciones mediante SafeExecute para robustez operacional.
// - Se implementaron validaciones de estado y modelo para garantizar integridad de datos.
//
// CONCLUSIONES
// 1. La implementación del patrón SafeExecute permite un manejo consistente de errores y mejor experiencia de usuario.
// 2. La estructura RESTful facilita la integración y mantenimiento del sistema de espacios.
// 3. La validación centralizada y el manejo de estados aseguran la consistencia de los datos en la base de datos.
// *************************************
using ProyectoDistri2.DAL;
using ProyectoDistri2.Models;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Http;

namespace ProyectoDistri2.WebAPI.Controllers
{
    [Authorize]
    [RoutePrefix("api/espacios")]
    public class EspaciosController : ApiController
    {
        private readonly GestorReserva db = new GestorReserva();

        // Método auxiliar para manejar excepciones
        private IHttpActionResult SafeExecute(System.Func<IHttpActionResult> action)
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
            catch (System.Exception ex)
            {
                return Content(HttpStatusCode.InternalServerError, new
                {
                    error = "Error inesperado en el servidor.",
                    detalle = ex.Message
                });
            }
        }

        // GET api/espacios
        [HttpGet]
        [AllowAnonymous]
        [Route("")]
        public IHttpActionResult GetEspacios() =>
            SafeExecute(() => Ok(db.Espacios.ToList()));

        // GET api/espacios/1
        [HttpGet]
        [Route("{id:int}")]
        public IHttpActionResult GetEspacio(int id) =>
            SafeExecute(() =>
            {
                var espacio = db.Espacios.Find(id);
                if (espacio == null) return NotFound();
                return Ok(espacio);
            });

        // POST api/espacios
        [HttpPost]
        [Route("")]
        public IHttpActionResult CrearEspacio(Espacio espacio) =>
            SafeExecute(() =>
            {
                if (!ModelState.IsValid) return BadRequest(ModelState);
                db.Espacios.Add(espacio);
                db.SaveChanges();
                return Ok(espacio);
            });

        // PUT api/espacios/1
        [HttpPut]
        [Route("{id:int}")]
        public IHttpActionResult ActualizarEspacio(int id, Espacio espacio) =>
            SafeExecute(() =>
            {
                if (!ModelState.IsValid || id != espacio.Id) return BadRequest();
                db.Entry(espacio).State = EntityState.Modified;
                db.SaveChanges();
                return StatusCode(HttpStatusCode.NoContent);
            });

        // DELETE api/espacios/1
        [HttpDelete]
        [Route("{id:int}")]
        public IHttpActionResult BorrarEspacio(int id) =>
            SafeExecute(() =>
            {
                var espacio = db.Espacios.Find(id);
                if (espacio == null) return NotFound();

                db.Espacios.Remove(espacio);
                db.SaveChanges();
                return Ok(espacio);
            });



    }
}
