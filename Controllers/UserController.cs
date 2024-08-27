using AspNetCoreHero.ToastNotification.Abstractions;
using Blog.ViewModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Blog.Controllers
{
    public class UserController : Controller
    {
        private SignInManager<IdentityUser> signInManager;
        private readonly ILogger<UserController> logger;
        private readonly INotyfService notyf;

        public UserController(SignInManager<IdentityUser> signInManager, ILogger<UserController> logger, INotyfService notyf)
        {
            this.signInManager = signInManager;
            this.logger = logger;
            this.notyf = notyf; 

        }

        [HttpGet]
        public async Task<IActionResult> Login()
        {
            try
            {
                await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);
            }
            catch (Exception ex)
            {               
                logger.LogError(ex, "Error signing out of external scheme");
                notyf.Error("Something went wrong, please contact admin or try again later!", 10);
            }

            
            return View("Login", new LoginViewModel());

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel vm)
        {
            if (!ModelState.IsValid)
            {
                return View(vm);
            }

            try
            {
                var result = await signInManager.PasswordSignInAsync(vm.UserName, vm.Password, vm.RememberMe, lockoutOnFailure: true);

                if (result.Succeeded)
                {
                    notyf.Success("Successfully logged in!", 3);
                    return RedirectToAction("Index", "Panel");
                }
                else if (result.IsLockedOut)
                {
                    logger.LogWarning("User account locked out.");
                    notyf.Error("Your account is locked out. Please try again later.", 5);
                }
                else if (result.IsNotAllowed)
                {
                    notyf.Error("You are not allowed to log in. Please check your account status.", 5);
                }
                else
                {
                    notyf.Error("Invalid login attempt. Please try again.", 5);
                    ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error during login attempt");
                notyf.Error("An unexpected error occurred. Please try again later.", 5);
                return View(vm);
            }
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await signInManager.SignOutAsync();
            notyf.Success("Successfully logged out", 3);
            return RedirectToAction("Index", "Home");
        }
    }
}
