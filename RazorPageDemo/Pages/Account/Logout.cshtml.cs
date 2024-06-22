using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace RazorPage.Pages.Account
{
    public class LogoutModel : PageModel
    {
        public async Task<IActionResult> OnGetAsync()
        {
            await HttpContext.SignOutAsync("MyCookie");
            return RedirectToPage("/Index");
        }

        //public IActionResult Logout()
        //{
            
        //    return RedirectToPage("/Account/Login");
        //}
    }
}
