using System;

namespace Common.Atribute
{
    public class MySpecialAttribute : Attribute
    {
        public Enums.TipoColumna Tipo;

        public MySpecialAttribute(Enums.TipoColumna tipo = Enums.TipoColumna.Texto)
        {
            Tipo = tipo;
        }
    }
}