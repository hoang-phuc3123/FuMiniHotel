using DataModel.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Repository;
using ViewModel;

namespace RazorPageDemo.Pages
{
    [Authorize]
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;

        private readonly CustomerViewModel _customerViewModel;

        [BindProperty]
        public List<BookingDetailViewModel> BookingHistory { get; set; }

        public IndexModel(CustomerViewModel customerViewModel, ILogger<IndexModel> logger)
        {
            _customerViewModel = customerViewModel;
            _logger = logger;
        }

        public async Task OnGetAsync()
        {
            string? customerId = User.FindFirst("CustomerId").Value;
            if (customerId != null)
            {
                BookingHistory = await _customerViewModel.GetListBookingHistoryAsync(customerId);
            }

        }
    }
}