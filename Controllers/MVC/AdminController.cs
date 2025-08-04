// *****************************************************
// Proyecto 2 - Reserva.co
// Controlador AdminController
// Kenneth Pantoja 
// Santiago Pila
// Fecha: 03/08/2025
//
// RESULTADOS FINALES
// - Se implementó panel de administración con acceso a múltiples APIs
// - Se integró validación de rol administrador para control de acceso
// - Se estableció comunicación segura con APIs de reservas, espacios y usuarios
// - Se implementó manejo de sesiones para persistencia de datos
// - Se centralizó la gestión de datos para el panel administrativo
//
// CONCLUSIONES
// 1. La integración con múltiples APIs permite una gestión centralizada de recursos
// 2. La validación de rol asegura la seguridad del panel administrativo
// 3. El manejo de sesiones facilita la persistencia de datos de usuario
// 4. La estructura del controlador permite una fácil extensión de funcionalidades
// *************************************

using Newtonsoft.Json;
using ProyectoDistri2.Models;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Mvc;

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

                // Obtener todas las reservas
                var reservasResp = await client.GetAsync(apiReservasUrl);
                var reservasJson = await reservasResp.Content.ReadAsStringAsync();
                var reservas = JsonConvert.DeserializeObject<List<Reserva>>(reservasJson);

                // Obtener todos los espacios
                var espaciosResp = await client.GetAsync(apiEspaciosUrl);
                var espaciosJson = await espaciosResp.Content.ReadAsStringAsync();
                var espacios = JsonConvert.DeserializeObject<List<Espacio>>(espaciosJson);

                // Obtener todos los usuarios
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
