using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;
using ViewModel;
using Microsoft.Identity.Client.Platforms.Features.DesktopOs.Kerberos;
using System.ComponentModel.DataAnnotations;
using DataModel.Models;

namespace RazorPage.Pages.Account
{
    public class Credential
    {
        [Required]
        public string Email { get; set; }
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        [Required]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Password and Confirm Password does not match")]
        public string ConfirmPassword { get; set; }
    }

    public class SignUpModel : PageModel
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly CustomerViewModel _customerViewModel;
        [BindProperty]
        public Credential credential { get; set; }
        public SignUpModel(IServiceProvider serviceProvider, CustomerViewModel customerViewModel)
        {
            _serviceProvider = serviceProvider;
            _customerViewModel = customerViewModel;
        }


        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync(Credential credential)
        {
            if(ModelState.IsValid)
            {
                var user = _customerViewModel.GetCustomerByEmail(credential.Email);
                if (user != null)
                {
                    ModelState.AddModelError(string.Empty, "Email has already existed.");
                    return Page();
                }

                Customer customer = new()
                {
                    EmailAddress = credential.Email,
                    CustomerFullName = "A",
                    Telephone = "1234567890",
                    CustomerBirthday = null,
                    CustomerStatus = 0,
                    Password = credential.Password,
                    EmailVerifyCode = new Random().Next(100000, 1000000)
                };

                EmailSendModel emailSendModel = new()
                {
                    ToEmail = credential.Email,
                    Body = customer.EmailVerifyCode.ToString(),
                    IsBodyHtml = true,
                    Subject = "Verify email"
                };

                _customerViewModel.RegisterCustomer(customer);

                TempData["Email"] = customer.EmailAddress;

                //var emailSenderService = _serviceProvider.GetRequiredService<EmailSenderService>();
                //emailSenderService.EnqueueEmail(emailSendModel);
                //emailSenderService.ExecuteTask();

            }
            return RedirectToPage("/Account/VerifyMail");
        }
    }
}
