using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Mvc;
using Newtonsoft.Json;
using System.Collections.Generic;
using ProyectoDistri2.Models;
using System;
using System.Linq;

namespace ProyectoDistri2.Controllers
{
    public class ReservasController : Controller
    {
        private readonly string apiEspaciosUrl = "https://localhost:44398/api/espacios";
        private readonly string apiReservasUrl = "https://localhost:44398/api/reservas";

        // GET: Reservas/Index
        public async Task<ActionResult> Index()
        {
            using (var client = new HttpClient())
            {
                // Agregar token JWT si está en Session
                if (Session["Token"] != null)
                {
                    client.DefaultRequestHeaders.Authorization =
                        new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", Session["Token"].ToString());
                }

                var response = await client.GetAsync(apiEspaciosUrl);
                if (!response.IsSuccessStatusCode)
                {
                    ViewBag.Error = "No se pudieron cargar los espacios disponibles.";
                    return View(new List<Espacio>());
                }

                var json = await response.Content.ReadAsStringAsync();
                var espacios = JsonConvert.DeserializeObject<List<Espacio>>(json);

                return View(espacios);
            }
        }

        // GET: Reservas/Crear/{espacioId}
        // GET: Reservas/Crear
        public ActionResult Crear(int espacioId)
        {
            var reserva = new Reserva
            {
                EspacioId = espacioId,
                FechaInicio = DateTime.Now,
                FechaFin = DateTime.Now.AddHours(1)
            };
            return View(reserva);
        }

        // POST: Reservas/Crear
        [HttpPost]
        public async Task<ActionResult> Crear(Reserva reserva)
        {
            // Asignar usuario y estado antes de validar
            if (Session["UserId"] == null)
                return Json(new { success = false, message = "Sesión expirada, vuelva a iniciar sesión." });

            reserva.UsuarioId = (int)Session["UserId"];
            reserva.Estado = "Pendiente";

            // Validación de fechas
            if (reserva.FechaInicio >= reserva.FechaFin)
                return Json(new { success = false, message = "La fecha de inicio debe ser menor a la fecha fin." });

            using (var client = new HttpClient())
            {
                if (Session["Token"] != null)
                {
                    client.DefaultRequestHeaders.Authorization =
                        new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", Session["Token"].ToString());
                }

                var response = await client.PostAsJsonAsync(apiReservasUrl, reserva);

                if (response.IsSuccessStatusCode)
                {
                    return Json(new { success = true, message = "Reserva creada correctamente. Pendiente de confirmación." });
                }

                // Leer error de la API (conflicto de horarios u otro)
                var error = await response.Content.ReadAsStringAsync();
                return Json(new { success = false, message = "No se pudo crear la reserva: " + error });
            }
        }



        // GET: Reservas/Historial
        public async Task<ActionResult> Historial()
        {
            if (Session["UserId"] == null)
            {
                return RedirectToAction("Login", "Account"); // 🔹 Redirige al login
            }

            int userId = (int)Session["UserId"];
            var reservas = await ObtenerHistorialUsuario();

            var espacios = await ObtenerEspacios();
            ViewBag.Espacios = espacios;

            return View(reservas);
        }


        // 🔹 Acción parcial para filtros AJAX
        [HttpGet]
        public async Task<ActionResult> FiltrarHistorial(
             DateTime? fechaInicio,
             DateTime? fechaFin,
             string estado = "",
             int? espacioId = null,
             int page = 1,
             int pageSize = 5
         )
        {
            var reservas = await ObtenerHistorialUsuario();

            if (fechaInicio.HasValue)
                reservas = reservas.FindAll(r => r.FechaInicio >= fechaInicio.Value);

            if (fechaFin.HasValue)
                reservas = reservas.FindAll(r => r.FechaFin <= fechaFin.Value);

            if (!string.IsNullOrEmpty(estado))
                reservas = reservas.FindAll(r => r.Estado == estado);

            if (espacioId.HasValue && espacioId.Value > 0)
                reservas = reservas.FindAll(r => r.EspacioId == espacioId.Value);

            reservas = reservas.OrderByDescending(r => r.FechaInicio).ToList();

            int totalRegistros = reservas.Count;
            var reservasPagina = reservas.Skip((page - 1) * pageSize).Take(pageSize).ToList();

            ViewBag.Page = page;
            ViewBag.PageSize = pageSize;
            ViewBag.TotalRegistros = totalRegistros;

            return PartialView("_HistorialTabla", reservasPagina);
        }


        // Función auxiliar para obtener historial del usuario logueado
        private async Task<List<Reserva>> ObtenerHistorialUsuario()
        {
            int userId = (int)Session["UserId"];
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization =
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", Session["Token"].ToString());

                string url = $"{apiReservasUrl}/historial/usuario/{userId}";
                var response = await client.GetAsync(url);

                if (!response.IsSuccessStatusCode) return new List<Reserva>();

                var json = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<List<Reserva>>(json);
            }
        }

        //Función auxiliar para obtener espacios
        private async Task<List<Espacio>> ObtenerEspacios()
        {
            using (var client = new HttpClient())
            {
                var response = await client.GetAsync(apiEspaciosUrl);
                if (!response.IsSuccessStatusCode) return new List<Espacio>();

                var json = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<List<Espacio>>(json);
            }
        }

        public async Task<ActionResult> ExportarHistorialExcel()
        {
            var reservas = await ObtenerHistorialUsuario();

            using (var workbook = new ClosedXML.Excel.XLWorkbook())
            {
                var ws = workbook.Worksheets.Add("Historial");
                ws.Cell(1, 1).Value = "ID";
                ws.Cell(1, 2).Value = "Espacio";
                ws.Cell(1, 3).Value = "Fecha Inicio";
                ws.Cell(1, 4).Value = "Fecha Fin";
                ws.Cell(1, 5).Value = "Estado";

                int row = 2;
                foreach (var r in reservas)
                {
                    ws.Cell(row, 1).Value = r.Id;
                    ws.Cell(row, 2).Value = r.Espacio?.Nombre;
                    ws.Cell(row, 3).Value = r.FechaInicio;
                    ws.Cell(row, 4).Value = r.FechaFin;
                    ws.Cell(row, 5).Value = r.Estado;
                    row++;
                }

                using (var stream = new System.IO.MemoryStream())
                {
                    workbook.SaveAs(stream);
                    stream.Position = 0;
                    return File(stream.ToArray(),
                                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                                "HistorialReservas.xlsx");
                }
            }
        }

        public async Task<ActionResult> PanelCoordinador()
        {
            // 🔹 Validar rol
            if ((string)Session["UserRole"] != "Coordinador" && (string)Session["UserRole"] != "Admin")
            {
                return RedirectToAction("Login", "Account");
            }

            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization =
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", Session["Token"].ToString());

                // Obtener reservas pendientes
                var response = await client.GetAsync("https://localhost:44398/api/reservas");
                if (!response.IsSuccessStatusCode)
                {
                    ViewBag.Error = "No se pudieron cargar las reservas.";
                    return View(new List<Reserva>());
                }

                var json = await response.Content.ReadAsStringAsync();
                var reservas = JsonConvert.DeserializeObject<List<Reserva>>(json);

                var pendientes = reservas.Where(r => r.Estado == "Pendiente").OrderBy(r => r.FechaInicio).ToList();

                // Obtener espacios para la segunda pestaña
                var responseEspacios = await client.GetAsync("https://localhost:44398/api/espacios");
                var jsonEspacios = await responseEspacios.Content.ReadAsStringAsync();
                var espacios = JsonConvert.DeserializeObject<List<Espacio>>(jsonEspacios);

                ViewBag.Espacios = espacios;
                return View(pendientes);
            }
        }

    }
}
