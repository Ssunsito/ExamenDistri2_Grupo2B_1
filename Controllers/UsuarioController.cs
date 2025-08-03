using System.Linq;
using System.Web.Http;
using ProyectoDistri2.DAL;
using System.Data.Entity;
using ProyectoDistri2.Models;
using System.Net;

namespace ProyectoDistri2.WebAPI.Controllers
{
    [Authorize(Roles = "Admin")]
    [RoutePrefix("api/usuarios")]
    public class UsuariosController : ApiController
    {
        private readonly GestorReserva db = new GestorReserva();

        // 🔹 Método auxiliar para manejar excepciones
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

        // GET api/usuarios
        [HttpGet]
        [AllowAnonymous] // Permitir listar usuarios públicamente
        [Route("")]
        public IHttpActionResult GetUsuarios() =>
            SafeExecute(() => Ok(db.Usuarios.ToList()));

        // GET api/usuarios/1
        [HttpGet]
        [Route("{id:int}")]
        public IHttpActionResult GetUsuario(int id) =>
            SafeExecute(() =>
            {
                var usuario = db.Usuarios.Find(id);
                if (usuario == null) return NotFound();
                return Ok(usuario);
            });

        // POST api/usuarios
        [HttpPost]
        [Route("")]
        public IHttpActionResult CrearUsuario(Usuario usuario) =>
            SafeExecute(() =>
            {
                if (!ModelState.IsValid) return BadRequest(ModelState);
                db.Usuarios.Add(usuario);
                db.SaveChanges();
                return Ok(usuario);
            });

        // PUT api/usuarios/1
        [HttpPut]
        [Route("{id:int}")]
        public IHttpActionResult ActualizarUsuario(int id, Usuario usuario) =>
            SafeExecute(() =>
            {
                if (!ModelState.IsValid || id != usuario.Id) return BadRequest();
                db.Entry(usuario).State = EntityState.Modified;
                db.SaveChanges();
                return StatusCode(HttpStatusCode.NoContent);
            });

        // DELETE api/usuarios/1
        [HttpDelete]
        [Route("{id:int}")]
        public IHttpActionResult BorrarUsuario(int id) =>
            SafeExecute(() =>
            {
                var usuario = db.Usuarios.Find(id);
                if (usuario == null) return NotFound();

                db.Usuarios.Remove(usuario);
                db.SaveChanges();
                return Ok(usuario);
            });
    }
}
