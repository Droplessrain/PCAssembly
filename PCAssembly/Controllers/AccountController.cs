using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using PCAssembly;
using PCAssembly.Models;

public class AccountController : Controller
{
    private readonly UserManager<User> _userManager;
    private readonly SignInManager<User> _signInManager;

    public AccountController(UserManager<User> userManager, SignInManager<User> signInManager)
    {
        _userManager = userManager;
        _signInManager = signInManager;
    }

    [HttpGet]
    public IActionResult Login()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginModel model)
    {
        if (ModelState.IsValid)
        {
            var result = await _signInManager.PasswordSignInAsync(model.UserName, model.Password, model.RememberMe, false);

            if (result.Succeeded)
            {
                var user = await _userManager.FindByNameAsync(model.UserName);
                var roles = await _userManager.GetRolesAsync(user);

                if (roles.Contains("Admin"))
                {
                    return RedirectToAction("Index", "Admin");
                }
                else if (roles.Contains("User"))
                {
                    return RedirectToAction("Profile", "User");
                }
            }

            ModelState.AddModelError("", "Неправильный логин или пароль");
        }
        return View(model);
    }


    [HttpGet]
    public IActionResult Register()
    {
        return View(new RegisterModel());
    }

    [HttpPost]
    public async Task<IActionResult> Register(RegisterModel model)
    {
        if (ModelState.IsValid)
        {
            var user = new User
            {
                UserName = model.Input.UserName,
                Email = model.Input.Email
            };

            var result = await _userManager.CreateAsync(user, model.Input.Password);
            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, "User");
                await _signInManager.SignInAsync(user, isPersistent: false);
                var roles = await _userManager.GetRolesAsync(user);
                if (roles.Contains("Admin"))
                {
                    return RedirectToAction("Index", "Admin");
                }
                else if (roles.Contains("User"))
                {
                    return RedirectToAction("Profile", "User");
                }
            }
            else
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }
        }
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();
        return RedirectToAction("Index", "Home");
    }
}