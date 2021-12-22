using System.Collections.Generic;

namespace Common
{
    public class DatosUsuario
    {
        public string Token { get; set; }
        public string Nombre { get; set; }
        public string Apellidos { get; set; }
        public string NombreCompleto => Nombre + " " + Apellidos;
        public string Avatar { get; set; }
        public string User { get; set; }
        public string IdUser { get; set; }
        public string Telefono { get; set; }
        public string Foto { get; set; }

       // public List<MenuItemsDto> Menu { get; set; }
        public List<string> Roles { get; set; }

        public DatosUsuario()
        {
          //  Menu = new List<MenuItemsDto>();
            Roles = new List<string>();
        }
    }
}