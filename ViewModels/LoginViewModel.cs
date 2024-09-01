using System.ComponentModel.DataAnnotations;
using YourProject.CustomAttributes;

namespace Blog.ViewModels
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Username is required")]
        [StringLength(18, MinimumLength = 5, ErrorMessage = "Username must be between 5 and 18 characters")]
        [Display(Name = "User")]
        public string UserName { get; set; } = null!;

       // [StrongPassword]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; } = null!;

        [Display(Name = "Remember me")]
        public bool RememberMe { get; set; } = false;
    }
}

