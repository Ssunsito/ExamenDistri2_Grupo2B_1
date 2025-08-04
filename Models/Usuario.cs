// *****************************************************
// Proyecto 2 – Reserva.co
// Modelo Clase Usuaerio
// Kenneth Pantoja
// Santiago Pila
// Fecha: 03/08/2025
//
// RESULTADOS
// - Se creó la entidad Usuario con propiedades Id, Nombre y Rol.
// - Se aplicaron [Required] para validar campos obligatorios.
//
// CONCLUSIONES
// - La clase permite gestionar usuarios y sus roles dentro del sistema.
// - Facilita la integración con Entity Framework y la autenticación básica.
// *****************************************************

using System.ComponentModel.DataAnnotations;

namespace ProyectoDistri2.Models
{
    public class Usuario
    {
        public int Id { get; set; }

        [Required]
        public string Nombre { get; set; }

        [Required]
        public string Rol { get; set; } // Admin, Coordinador, Profesor

        [Required]
        public string Password { get; set; } // 🔹 Contraseña para login
    }
}
