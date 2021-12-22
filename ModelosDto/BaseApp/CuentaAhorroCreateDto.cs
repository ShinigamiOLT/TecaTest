using System;

namespace ModelosDto
{
    public class CuentaAhorroCreateDto
    {
  
        public int NumeroCuenta { get; set; }
        public DateTime FechaCreacion { get; set; }
        public decimal SaldoActual { get; set; }
    //    public int ClienteId { get; set; }
    }
}