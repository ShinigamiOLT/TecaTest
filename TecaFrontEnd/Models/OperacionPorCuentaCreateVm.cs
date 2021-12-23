using System;
using System.ComponentModel.DataAnnotations;

namespace TecaFrontEnd.Models
{
    public class OperacionPorCuentaCreateVm
    {
       
        [Display(Name = "Tipo Operacion")]
        public int TipoOperacion { get; set; }
        [Display(Name = "Monto Transacción")]
        [Required]
        [Range(0.01, Double.MaxValue, ErrorMessage = "El campo {0} debe ser mayor que {1}.")]
        public decimal MontoTransaccion { get; set; }
      
        public int CuentaId { get; set; }

    }
}
