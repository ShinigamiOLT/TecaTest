using System;
using System.Collections.Generic;
using Common;
using Microsoft.AspNetCore.Identity;


namespace Modelos.Identity
{
    public class ApplicationUser : IdentityUser
    {
        public string Nombre { get; set; }

        public string Apellido { get; set; }
        public string Avatar { get; set; }
        public DateTime?   FechaAlta { get; set; }

        public List<ApplicationUserRole> UserRoles { get; set; }

        //public List<Tarea> Tareas { get; set; }

        public Enums.Borrado EsBorrado { get; set; }
    }
}