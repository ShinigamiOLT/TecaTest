using Common;
using System;
using System.ComponentModel.DataAnnotations;

namespace TecaFrontEnd.Models
{
    public class ErrorViewModel
    {
        public string RequestId { get; set; }

        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
    }
    public class ClienteVM
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
    public class CuentaAhorroVm
    {
        public int Id { get; set; }
        [Display(Name = "Numero Cuenta")]
        public int NumeroCuenta { get; set; }
        public DateTime FechaCreacion { get; set; }
        [Display(Name = "Saldo Actual")]
        public decimal SaldoActual { get; set; }
        public int ClienteId { get; set; }
    }
    public class OperacionPorCuentaVm
    {
        public int Id { get; set; }
        public DateTime FechaOperacion { get; set; }
   
        public string _TipoOperacion { get; set; }
         [Display(Name = "Tipo Operacion")]    
        public int TipoOperacion { get; set; }
        public decimal SaldoInicial { get; set; }
        [Display(Name = "Monto Transacción")]
        public decimal MontoTransaccion { get; set; }
        public decimal SaldoFinal { get; set; }
        public int CuentaId { get; set; }


    }
}
