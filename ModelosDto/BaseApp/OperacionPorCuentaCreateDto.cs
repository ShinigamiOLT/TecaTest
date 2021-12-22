using System;
using Common;

namespace ModelosDto
{
    public class OperacionPorCuentaCreateDto
    {
  
    //    public DateTime FechaOperacion { get; set; }
        public Enums.TipoOperacion TipoOperacion { get; set; }
      //  public decimal SaldoInicial { get; set; }
        public decimal MontoTransaccion { get; set; }
      //  public decimal SaldoFinal { get; set; }

    }
}