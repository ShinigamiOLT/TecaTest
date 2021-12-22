using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Runtime.CompilerServices;

namespace Common
{
    public class ApplicationUserRegisterDto  : ApplicationUserLoginDto
    {
       

  
        [Required(AllowEmptyStrings = false)]
        public string Apellido { get; set; }

        [Required(AllowEmptyStrings = false)]
        public string Nombre { get; set; }

        public string Telefono { get; set; }

    

        public ApplicationUserRegisterDto()
        {
            Plantilla = new Plantilla();
        }
    }
}