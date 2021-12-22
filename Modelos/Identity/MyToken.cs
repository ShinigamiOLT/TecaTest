using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Modelos.Identity
{
    public class MyToken
    {
        [Key]
        public string MyKey { get; set; }

        public string Code { get; set; }

        public string UserId { get; set; }
        public DateTime? DateExpired { get; set; }
        public string Tipo { get; set; }

        [ForeignKey("UserId")]
        public ApplicationUser User { get; set; }
    }
}
