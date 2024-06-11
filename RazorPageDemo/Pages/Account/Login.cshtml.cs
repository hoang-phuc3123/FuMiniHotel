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
        private readonly CustomerViewModel _customerViewModel;

        [BindProperty]
        public Credential credential { get; set; }

        public LoginModel(CustomerViewModel customerViewModel)
        {
            _customerViewModel = customerViewModel;
        }

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (ModelState.IsValid)
            {
                var user = _customerViewModel.GetCustomerByEmailAndPassword(credential.Email, credential.Password);
                if (user != null)
                {
                    var claims = new List<Claim>
                    {
                        new Claim("CustomerId",user.Id.ToString()),
                        new Claim(ClaimTypes.Name, user.Name),
                        new Claim(ClaimTypes.Email, user.Email)
                    };

                    var claimsIdentity = new ClaimsIdentity(claims, "MyCookie");
                    ClaimsPrincipal claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

                    await HttpContext.SignInAsync("MyCookie",claimsPrincipal);

                    return RedirectToPage("/Index");
                }

                ModelState.AddModelError(string.Empty, "Invalid login attempt.");
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
