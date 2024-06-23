using DataModel.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using ViewModel;

namespace RazorPage.Pages
{
    [Authorize(Roles = "Customer")]
    public class AccountModel : PageModel
    {
        private readonly CustomerViewModel _customerViewModel;

        [BindProperty]
        public CustomerProfile Customer { get; set; }

        public AccountModel(CustomerViewModel customerViewModel)
        {
            _customerViewModel = customerViewModel;
        }

        public async Task OnGetAsync()
        {
            Customer = await _customerViewModel.GetCustomerProfileAsync(Int32.Parse(User.FindFirst("CustomerId").Value));
        }
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var updateResult = await _customerViewModel.UpdateCustomerProfileAsync(Customer);
            if (!updateResult)
            {
                ModelState.AddModelError(string.Empty, "Failed to update profile. Please try again.");
                return Page();
            }
            TempData["SuccessMessage"] = "Profile updated successfully.";
            return Page();
            //return RedirectToPage("Customers/Profile");
        }
    }
   
}
