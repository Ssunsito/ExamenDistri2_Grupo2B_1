// *****************************************************
// Proyecto 2 – Reserva.co
// Modelo Clase Reserva
// Kenneth Pantoja
// Santiago Pila
// Fecha: 03/08/2025
//
// RESULTADOS
// - Se creó la entidad Reserva con propiedades para usuario, espacio y fechas.
// - Se aplicaron [Required] y [ForeignKey] para validar y relacionar entidades.
//
// CONCLUSIONES
// - La clase permite gestionar reservas con control de estados.
// - La relación con Usuario y Espacio facilita la integración con Entity Framework.
// *****************************************************
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
