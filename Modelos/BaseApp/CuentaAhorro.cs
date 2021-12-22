using Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Modelos.BaseApp
{
    public class CuentaAhorro :EntidadDelete
    {
        public int Id { get; set; }
        public int NumeroCuenta { get; set; }
        public DateTime FechaCreacion { get; set; }
        public decimal SaldoActual { get; set; }
        public int ClienteId { get; set; }
        [ForeignKey("ClienteId")]
        public Cliente Cliente { get; set; }

        public List<OperacionPorCuenta> Operaciones { get; set; }
    }
}