using System.ComponentModel;

namespace Common
{
    public class EntidadDelete
    {
        [DefaultValue(Enums.Visibilidad.Visible)]
        public Enums.Visibilidad Visible { get; set; }

        [DefaultValue(Enums.Borrado.Existe)] public Enums.Borrado EsBorrado { get; set; }
    }
}