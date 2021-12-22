using System.ComponentModel.DataAnnotations;

namespace Common
{
    public class ApplicationUserLoginDto
    {
        [Required(AllowEmptyStrings = false)]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }

        public Plantilla Plantilla { get; set; }

        public ApplicationUserLoginDto()
        {
            Plantilla = new Plantilla();
        }
    }
}