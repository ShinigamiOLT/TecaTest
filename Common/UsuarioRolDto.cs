using System.Collections.Generic;

namespace Common
{
    public class UsuarioRolDto
    {
        public string UserId { get; set; }
        public string Nombre { get; set; }
        public string Email { get; set; }

        public List<string> Roles { get; set; } = new List<string>();
    }
}