using System;
using System.ComponentModel.DataAnnotations;

namespace TecaFrontEnd.Models
{
    public class CuentaAhorroVm
    {
        public int Id { get; set; }
        [Display(Name = "Numero Cuenta")]
        [Required(ErrorMessage = "Campo requerido {0}")]
        [Range(1, int.MaxValue, ErrorMessage = "El campo {0} debe ser mayor/igual que {1}.")]
        public int NumeroCuenta { get; set; }
        public DateTime FechaCreacion { get; set; }
        [Display(Name = "Saldo Actual")]

        [Range(0.0, Double.MaxValue, ErrorMessage = "El campo {0} debe ser mayor/igual que {1}.")]
        public decimal SaldoActual { get; set; }
        public int ClienteId { get; set; }
    }
}
