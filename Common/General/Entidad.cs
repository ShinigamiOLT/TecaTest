using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public abstract class Entidad : EntidadDelete
    {
        public string UsuarioCreacion { get; set; }

        public DateTimeOffset FechaCreacion { get; set; }
        public string UsuarioModificacion { get; set; }
        public DateTimeOffset FechaModificacion { get; set; }
    }
}
