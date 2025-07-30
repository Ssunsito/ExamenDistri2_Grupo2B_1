using System.ComponentModel.DataAnnotations;

namespace ProyectoDistri2.Models
{
    public class Espacio
    {
        public int Id { get; set; }

        [Required]
        public string Nombre { get; set; }

        [Required]
        public string Tipo { get; set; } // Aula, Laboratorio, Auditorio
    }
}
