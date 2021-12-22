using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Common;
using Common.Atribute;
using System.Linq;

namespace ModelosDto
{
    public class UsuariosPerfilDto
    {
        public string Id { get; set; }
        public int Numero { get; set; }
        [NoVisible] public string Nombre { get; set; }
        public string Nombre_ { get; set; }
        public string Apellidos { get; set; }
        [NoVisible] public Enums.Visibilidad Visible { get; set; }
        [NoVisible] public DateTime Fecha { get; set; }
        public string Correo { get; set; }
        public string Avatar { get; set; }
        public List<string> Roles { get; set; }
        public string Fecha_ => Fecha.ToUSA();

       
  
        [NoVisible] public decimal SalarioBase { get; set; }

        public UsuariosPerfilDto()
        {
           
        }

        [NoVisible()] public Enums.EstadoTrabajador Estado { get; set; }
        public string Estatus => MyStado(Estado);

        public static string MyStado(Enums.EstadoTrabajador Estado)
        {
            string e = "Desconocido";
            switch (Estado)
            {
                case Enums.EstadoTrabajador.Activo:
                    return "Activo";

                case Enums.EstadoTrabajador.Inactivo:
                    return "Inactivo";
            }

            return e;
        }
    }
}