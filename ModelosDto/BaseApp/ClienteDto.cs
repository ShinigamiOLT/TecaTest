using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;


namespace ModelosDto
{
    public class ClienteDto
    {
        public int Id { get; set; }
        [Required]
        public string NumeroIdentificacion { get; set; }
        [Required]
        public string Nombre { get; set; }
     
        [Required] 
        public string ApellidoPaterno { get; set; }
        [Required] 
        public string ApellidoMaterno { get; set; }
        public DateTime FechaCreacion { get; set; }  
        public string Apellidos => $"{ApellidoPaterno} {ApellidoMaterno}" ;
    }
}