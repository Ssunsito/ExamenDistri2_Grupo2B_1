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
    }
}
