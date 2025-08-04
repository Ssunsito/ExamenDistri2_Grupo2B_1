// *****************************************************
//Proyecto 2 – Reserva.co
// Modelo Clase Espacio
// Kenneth Pantoja
// Santiago Pila
// Fecha: 03/08/2025
//
// RESULTADOS
// - Se creó la entidad Espacio con propiedades Id, Nombre y Tipo.
// - Se aplicaron [Required] para validar campos obligatorios.
//
// CONCLUSIONES
// - Facilita la integración con Entity Framework.
// - La validación evita registros incompletos.
// *****************************************************

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
