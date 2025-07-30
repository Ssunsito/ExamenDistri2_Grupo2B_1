using System;
using System.Web.Http;
using System.Data.Entity;
using ProyectoDistri2.Negocio;

namespace ProyectoDistri2.WebAPI.Controllers
{
    
    [RoutePrefix("api/reservas")]
    public class ReservasController : ApiController
    {
        private readonly ReservaBN service = new ReservaBN();

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
    }
}
