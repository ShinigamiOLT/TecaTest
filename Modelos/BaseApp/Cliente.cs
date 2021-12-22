using Common;
using System;
using System.Collections.Generic;

namespace Modelos.BaseApp
{
    public class Cliente : EntidadDelete
    {
        public int Id { get; set; }
        public string NumeroIdentificacion { get; set; }
        public string Nombre { get; set; }  
        public string ApellidoPaterno { get; set; }
        public string ApellidoMaterno { get; set; }
        public DateTime FechaCreacion { get; set; }
        public List<CuentaAhorro> Cuentas { get; set; }
    }
}