using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;
using ViewModel;
using Microsoft.Identity.Client.Platforms.Features.DesktopOs.Kerberos;
using System.ComponentModel.DataAnnotations;

namespace RazorPage.Pages.Account
{
    public class LoginModel : PageModel
    {
        private readonly IConfiguration _configuration;
        private readonly CustomerViewModel _customerViewModel;
        private readonly EmailViewModel _emailViewModel;

        [BindProperty]
        public Credential credential { get; set; }

        public LoginModel(CustomerViewModel customerViewModel, IConfiguration configuration, EmailViewModel emailViewModel)
        {
            _customerViewModel = customerViewModel;
            _configuration = configuration;
            _emailViewModel = emailViewModel;
        }

        public void OnGet()
        {
            
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var EMAIL_ADMIN = _configuration["Admin:Email"];
            var PASSWORD_ADMIN = _configuration["Admin:Password"];
            if (ModelState.IsValid)
            {
                var admin = false;
                var claims = new List<Claim>();
                if (credential.Email == EMAIL_ADMIN && credential.Password == PASSWORD_ADMIN)
                {
                    claims = new List<Claim>
                    {
                        new Claim("AdminId",EMAIL_ADMIN.ToString()),
                        new Claim(ClaimTypes.Email, EMAIL_ADMIN),
                        new Claim(ClaimTypes.Role, "Admin")
                    };
                    admin = true;
                }
                else
                {
                    var user = _customerViewModel.GetCustomerByEmailAndPassword(credential.Email, credential.Password);
                    if (user == null)
                    {
                        ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                        return Page();
                    }
                    claims = new List<Claim>
                    {
                        new Claim("CustomerId",user.Id.ToString()),
                        new Claim(ClaimTypes.Name, user.Name),
                        new Claim(ClaimTypes.Email, user.Email),
                        new Claim(ClaimTypes.Role, "Customer")
                    };
                }
                var claimsIdentity = new ClaimsIdentity(claims, "MyCookie");
                ClaimsPrincipal claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

                await HttpContext.SignInAsync("MyCookie", claimsPrincipal);
                if (admin == true)
                {
                    return RedirectToPage("/Admin/Admin");
                }
                else
                {
                    return RedirectToPage("/Index");
                } 
            }
            return Page();
        }
        public class Credential
        {
            [Required]
            public string Email { get; set; }
            [Required]
            [DataType(DataType.Password)]
            public string Password { get; set; }
        }
    }
}
