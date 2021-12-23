using System;
using System.ComponentModel.DataAnnotations;

namespace TecaFrontEnd.Models
{
    public class ClienteVm
    {
        public int Id { get; set; }
        [Required]
        [Display(Name ="Numero Identificación")]
        public string NumeroIdentificacion { get; set; }
        [Required]
        public string Nombre { get; set; }

        [Required]
        [Display(Name = "Apellido Paterno")]
        public string ApellidoPaterno { get; set; }
        [Required]
        [Display(Name = "Apellido Materno")]
        public string ApellidoMaterno { get; set; }
        [Display(Name = "Fecha Creación")]
        public DateTime FechaCreacion { get; set; }
        public string Apellidos => $"{ApellidoPaterno} {ApellidoMaterno}";
    }
}
