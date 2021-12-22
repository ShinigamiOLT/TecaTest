using Common;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Modelos.BaseApp
{
    public class OperacionPorCuenta
    {
        public int Id { get; set; }
        public DateTime FechaOperacion { get; set; }
        public Enums.TipoOperacion TipoOperacion {get;set;}
        public decimal SaldoInicial { get; set; }
        public decimal MontoTransaccion { get; set; }
        public decimal SaldoFinal { get; set; }
        public int CuentaId { get; set; }
        [ForeignKey("CuentaId")]
        public CuentaAhorro Cuenta { get; set; }
    }
}