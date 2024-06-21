using DataModel.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using ViewModel;

namespace RazorPage.Pages.Account
{
    public class Verify
    {
        [Required]
        public string Email { get; set; }
        [Required]
        public string VerifyCode { get; set; }
    }
    public class VerifyMailModel : PageModel
    {
        private readonly CustomerViewModel _customerViewModel;
        [BindProperty]
        public Verify Verify { get; set; }
        public VerifyMailModel(CustomerViewModel customerViewModel)
        {
            _customerViewModel = customerViewModel;
        }
        //public async Task<IActionResult> OnGetAsync()
        //{
        //    //ViewData["Email"] = TempData["Email"] as string;
        //    return Page();
        //}

        public void OnGet()
        {
            if (TempData.ContainsKey("Email"))
            {
                Verify = new Verify
                {
                    Email = TempData["Email"].ToString()
                };
            }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (ModelState.IsValid)
            {
                var customer = _customerViewModel.GetCustomerByEmail(Verify.Email);
                if (customer == null)
                {
                    ModelState.AddModelError(string.Empty, "Customer not found");
                    return Page();
                }

                if(!customer.EmailVerifyCode.ToString().Equals(Verify.VerifyCode)) 
                {
                    ModelState.AddModelError(string.Empty, "Verify code is wrong!");
                    return Page();
                }

                customer.CustomerStatus = 1;
                _customerViewModel.UpdateCustomer(customer);

                ModelState.AddModelError(string.Empty, "Successfully, login now.");
            }
            return Page();
        }
    }
}
