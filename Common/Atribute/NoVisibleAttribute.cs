using System;

namespace Common.Atribute
{
    public class NoVisibleAttribute : Attribute
    {
        public Enums.Visibilidad Tipo;

        //  public NoVisibleAttribute()
        public NoVisibleAttribute(Enums.Visibilidad tipo = Enums.Visibilidad.Oculto)
        {
            Tipo = tipo; //Enums.Visibilidad.Oculto;
        }
    }
}