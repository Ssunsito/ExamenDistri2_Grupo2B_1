using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Mvc;
using Newtonsoft.Json;
using System.Collections.Generic;
using ProyectoDistri2.Models;

namespace ProyectoDistri2.Controllers
{
    public class AdminController : Controller
    {
        private readonly string apiReservasUrl = "https://localhost:44398/api/reservas";
        private readonly string apiEspaciosUrl = "https://localhost:44398/api/espacios";
        private readonly string apiUsuariosUrl = "https://localhost:44398/api/usuarios";

        // GET: Admin/Panel
        public async Task<ActionResult> Panel()
        {
            // Validar que sea Admin
            if ((string)Session["UserRole"] != "Admin")
            {
                return RedirectToAction("Login", "Account");
            }

            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization =
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", Session["Token"].ToString());

                // 🔹 Obtener todas las reservas
                var reservasResp = await client.GetAsync(apiReservasUrl);
                var reservasJson = await reservasResp.Content.ReadAsStringAsync();
                var reservas = JsonConvert.DeserializeObject<List<Reserva>>(reservasJson);

                // 🔹 Obtener todos los espacios
                var espaciosResp = await client.GetAsync(apiEspaciosUrl);
                var espaciosJson = await espaciosResp.Content.ReadAsStringAsync();
                var espacios = JsonConvert.DeserializeObject<List<Espacio>>(espaciosJson);

                // 🔹 Obtener todos los usuarios
                var usuariosResp = await client.GetAsync(apiUsuariosUrl);
                var usuariosJson = await usuariosResp.Content.ReadAsStringAsync();
                var usuarios = JsonConvert.DeserializeObject<List<Usuario>>(usuariosJson);

                // Enviar listas por ViewBag
                ViewBag.Espacios = espacios;
                ViewBag.Usuarios = usuarios;
                return View(reservas);
            }
        }
    }
}
