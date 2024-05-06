using ContactsManager.Core.Domain.Entities.IdentityEntities;
using ContactsManager.Core.DTO;
using ContactsManager.Core.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace ContactsManager.UI.Controllers
{
    [Route("[Controller]/[Action]")]
    // [AllowAnonymous]
    [Authorize(Policy = "NotAuthrized")]
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser>? _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager; 
        private readonly RoleManager<ApplicationRole> _roleManager; 
        public AccountController(UserManager<ApplicationUser>? userManager , SignInManager<ApplicationUser> signInManager
            ,RoleManager<ApplicationRole> roleManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
        }

        public IActionResult Register()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult>Register(RegisterDTO registerDTO)
        {
            if(!ModelState.IsValid)
            {
                ViewBag.Errors = ModelState.Values.SelectMany(temp => temp.Errors).Select(t => t.ErrorMessage);
                return View(registerDTO);
            }
            ApplicationUser applicationUser = new ApplicationUser()
            {
                Email = registerDTO.Email,
                UserName = registerDTO.Email,
                PhoneNumber = registerDTO.Phone,
                PersonName = registerDTO.PersonName
            };
            IdentityResult? identityResult= await _userManager.CreateAsync(applicationUser, registerDTO.Password);
            if(identityResult.Succeeded)
            {
                if(await _roleManager.FindByNameAsync(registerDTO.Role.ToString())==null)
                {
                    ApplicationRole role = new ApplicationRole() { Name=registerDTO.Role.ToString()};
                    await _roleManager.CreateAsync(role);
                }
                await _userManager.AddToRoleAsync(applicationUser, registerDTO.Role.ToString());
                await _signInManager.SignInAsync(applicationUser, false);
                return RedirectToAction("Index", "Contacts");
            }
            else
            {
                foreach(IdentityError identityError in  identityResult.Errors)
                {
                    ModelState.AddModelError("Register", identityError.Description);
                }
                return View();
            }
        }
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
       // [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginDTO loginDTO ,string? ReturnUrl)
        {
            if(!ModelState.IsValid)
            {
                ViewBag.Errors=ModelState.Values.SelectMany(t=>t.Errors).Select(t => t.ErrorMessage);
                return View(loginDTO);
            }
            Microsoft.AspNetCore.Identity.SignInResult? signInResult= await _signInManager.PasswordSignInAsync(loginDTO.Email, loginDTO.Password,false,false);
            if(signInResult.Succeeded)
            {
                ApplicationUser? user=await _userManager.FindByEmailAsync(loginDTO.Email);
                if(await _userManager.IsInRoleAsync(user,Roles.Admin.ToString()))
                {
                    return RedirectToAction("Index", "Home", new { area="Admin"});
                }
                if(!string.IsNullOrEmpty(ReturnUrl)&&Url.IsLocalUrl(ReturnUrl))
                {
                    return LocalRedirect(ReturnUrl);
                }
                return RedirectToAction("Index", "Contacts");
            }
            ModelState.AddModelError("Login", "Invalid Email or Password");
            return View(loginDTO);
        }
        [AllowAnonymous]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index","Contacts");
        }
        [AllowAnonymous]
        public async Task<IActionResult>IsEmailUsed(string Email)
        {
            return await _userManager.FindByEmailAsync(Email)!=null? Json(false) : Json(true);
        }
    }
}
