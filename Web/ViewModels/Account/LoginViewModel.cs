using System.ComponentModel.DataAnnotations;

namespace MediaCommMvc.Web.ViewModels.Account
{
    public class LoginViewModel
    {
        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [Display(Name = "Remember me?")]
        public bool RememberMe { get; set; }

        [Required]
        [Display(Name = "User name")]
        public string UserName { get; set; }
    }
}