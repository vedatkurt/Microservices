using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace FreeCourse.Web.Models
{
    public class SigninInput
    {
        [Required]

        [Display(Name ="Email adresiniz")]
        public string Email { get; set; }

        [Display(Name = "Sifreniz")]
        public string Password { get; set; }

        [Display(Name = "Beni hatirla")]
        public bool IsRemember { get; set; }
    }
}
