// *****************************************************
//Examen 2 – Clase Espacio
// Kenneth Andrés Pantoja Manobanda
// Santiago Pila
// Fecha: 30/07/2025
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
