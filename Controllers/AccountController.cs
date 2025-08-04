// *****************************************************
// Proyecto Distri2 - Sistema de Gestión de Reservas
// Controlador AccountController
// Kenneth Pantoja 
// Santiago Pila
// Fecha: 03/08/2025
//
// RESULTADOS FINALES
// - Se implementó sistema de autenticación JWT con tokens Bearer
// - Se integró gestión de sesiones para almacenamiento de datos de usuario
// - Se implementó redirección condicional basada en roles de usuario
// - Se centralizó el manejo de errores de autenticación
// - Se estableció comunicación segura con API de usuarios
//
// CONCLUSIONES
// 1. La implementación de JWT proporciona una autenticación segura y escalable
// 2. El manejo de sesiones permite mantener el estado del usuario de forma eficiente
// 3. La gestión de roles asegura el acceso controlado a funcionalidades específicas
// *************************************

using Newtonsoft.Json;
using ProyectoDistri2.Models;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace ProyectoDistri2.Controllers
{
    public class AccountController : Controller
    {
        private readonly string apiTokenUrl = "https://localhost:44398/token";
        private readonly string apiUsuariosUrl = "https://localhost:44398/api/usuarios";

        // GET: Account/Login
        public ActionResult Login()
        {
            return View();
        }

        // POST: Account/Login
        [HttpPost]
        public async Task<ActionResult> Login(string username, string password)
        {
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                ViewBag.Error = "Debe ingresar usuario y contraseña.";
                return View();
            }

            using (var client = new HttpClient())
            {
                // Solicitud de token JWT
                var form = new FormUrlEncodedContent(new[]
                {
                    new KeyValuePair<string,string>("grant_type", "password"),
                    new KeyValuePair<string,string>("username", username),
                    new KeyValuePair<string,string>("password", password)
                });

                var response = await client.PostAsync(apiTokenUrl, form);

                if (!response.IsSuccessStatusCode)
                {
                    ViewBag.Error = "Usuario o contraseña incorrectos.";
                    return View();
                }

                // Leer token JWT
                var json = await response.Content.ReadAsStringAsync();
                dynamic tokenResponse = JsonConvert.DeserializeObject(json);

                // Guardar token en Session
                Session["Token"] = (string)tokenResponse.access_token;
                Session["UserName"] = username;

                // Obtener usuario desde la API
                var user = await ObtenerUsuario(username);
                if (user == null)
                {
                    ViewBag.Error = "No se pudo obtener la información del usuario.";
                    return View();
                }

                // Guardar datos en Session
                Session["UserId"] = user.Id;
                Session["UserRole"] = user.Rol;

                // Redirección según rol
                switch (user.Rol)
                {
                    case "Coordinador":
                        return RedirectToAction("PanelCoordinador", "Reservas");
                    case "Admin":
                        return RedirectToAction("Panel", "Admin"); // Nuevo panel admin
                    default: // Profesor
                        return RedirectToAction("Index", "Reservas");
                }
            }
        }

        // Logout
        public ActionResult Logout()
        {
            Session.Clear();
            return RedirectToAction("Login");
        }

        // Obtener usuario desde la API
        private async Task<Usuario> ObtenerUsuario(string username)
        {
            using (var client = new HttpClient())
            {
                if (Session["Token"] != null)
                {
                    client.DefaultRequestHeaders.Authorization =
                        new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", Session["Token"].ToString());
                }

                var response = await client.GetAsync(apiUsuariosUrl);
                if (!response.IsSuccessStatusCode) return null;

                var json = await response.Content.ReadAsStringAsync();
                var usuarios = JsonConvert.DeserializeObject<List<Usuario>>(json);

                return usuarios.FirstOrDefault(u => u.Nombre == username);
            }
        }
    }
}
