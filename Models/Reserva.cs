using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProyectoDistri2.Models
{
    public class Reserva
    {
        public int Id { get; set; }

        [Required]
        public int UsuarioId { get; set; }

        [Required]
        public int EspacioId { get; set; }

        [Required]
        public DateTime FechaInicio { get; set; }

        [Required]
        public DateTime FechaFin { get; set; }

        [Required]
        public string Estado { get; set; } // Pendiente, Aprobada, Rechazada

        [ForeignKey("UsuarioId")]
        public virtual Usuario Usuario { get; set; }

        [ForeignKey("EspacioId")]
        public virtual Espacio Espacio { get; set; }
    }
}
