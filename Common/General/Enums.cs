using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;

namespace Common
{
    public class Enums
    {
        public enum TipoUser
        {
            All,
            Admin,
            NoAdmin
        }




        public enum EstadoTrabajador
        {
            Activo,
            Inactivo
        }


        public enum Borrado
        {
            Existe,
            Borrado
        }


        public enum Visibilidad
        {
            Visible,
            Oculto
        }
        public enum TipoColumna
        {
            Texto = 1,
            Parrafo = 2,
            Lista = 3,
            More = 4,

            File = 5,
            Boolean = 6
        }





        public enum TipoPestania
        {
            Tabla,
            Link
        }





        public enum TipoOperacion
        {
            Deposito,
            Retiro

        }
    }


    public static class EnumsToString
    {
     


    
        public static string To(Enums.TipoOperacion tipo)
        {
            return tipo switch
            {
                Enums.TipoOperacion.Retiro => "Retiro",
                Enums.TipoOperacion.Deposito => "Deposito",
                _ => ""
            };
        }

        public static string Generico(object tipo)
        {
          
            if (tipo is Enums.TipoOperacion tipooperacion)
                return To(tipooperacion);
            return "";
        }

       
    }
}