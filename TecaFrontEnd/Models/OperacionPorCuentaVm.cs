using System;
using System.ComponentModel.DataAnnotations;

namespace TecaFrontEnd.Models
{
    public class OperacionPorCuentaVm
    {
        public int Id { get; set; }
        public DateTime FechaOperacion { get; set; }
   
        public string _TipoOperacion { get; set; }
         [Display(Name = "Tipo Operacion")]    
        public int TipoOperacion { get; set; }
        public decimal SaldoInicial { get; set; }
        [Display(Name = "Monto Transacción")]
  
        [Range(0.0, Double.MaxValue, ErrorMessage = "El campo {0} debe ser mayor que {1}.")]
        public decimal MontoTransaccion { get; set; }
        public string MontoTransaccion_=> $"{MontoTransaccion:C}";
        public decimal SaldoFinal { get; set; }
        public int CuentaId { get; set; }

    }
}
