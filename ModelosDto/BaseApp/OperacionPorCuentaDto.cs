using System;
using Common;

namespace ModelosDto
{
    public class OperacionPorCuentaDto
    {
        public int Id { get; set; }
        public DateTime FechaOperacion { get; set; }        
        public string _TipoOperacion { get; set; }
        public decimal SaldoInicial { get; set; }
        public decimal MontoTransaccion { get; set; }
        public decimal SaldoFinal { get; set; }
      
     
    }
}