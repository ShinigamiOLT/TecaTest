using System;
using System.ComponentModel.DataAnnotations;

namespace TecaFrontEnd.Models
{
    public class CuentaAhorroCreateVm
    {
      
        [Display(Name = "Numero Cuenta")]
        [Required (ErrorMessage ="Campo requerido {0}")]
        [Range(1, int.MaxValue, ErrorMessage = "El campo {0} debe ser mayor/igual que {1}.")]
        public int NumeroCuenta { get; set; }
      
        [Display(Name = "Saldo Actual")]
        [Required]
        [Range(0.0, Double.MaxValue, ErrorMessage = "El campo {0} debe ser mayor/igual que {1}.")]
        public decimal SaldoActual { get; set; }
        public int ClienteId { get; set; }
    }
}
