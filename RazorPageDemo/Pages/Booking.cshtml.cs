using DataModel.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using ViewModel;

namespace RazorPage.Pages
{
    public class BookingModel : PageModel
    {
        private readonly CustomerViewModel _customerViewModel;

        [BindProperty]
        public List<RoomViewModel> Rooms { get; set; }

        [BindProperty]
        public DateTime dtpStartDate { get; set; }

        [BindProperty]
        public DateTime dtpEndDate { get; set; }

        [BindProperty(SupportsGet = true)]
        public List<string> SelectedRooms { get; set; }

        public BookingModel(CustomerViewModel customerViewModel)
        {
            _customerViewModel = customerViewModel;
            dtpStartDate = DateTime.Today;
            dtpEndDate = DateTime.Today.AddDays(1);
        }

        public async Task OnGetAsync()
        {
            Rooms = await _customerViewModel.GetAvailableRooms();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!IsValidData())
            {
                ModelState.AddModelError(string.Empty, "Invalid data. Please check your entries.");

                // Reload the rooms data to ensure the table is populated
                Rooms = await _customerViewModel.GetAvailableRooms();

                return Page();
            }

            var isSuccess = await _customerViewModel.BookRooms(Int32.Parse(User.FindFirst("CustomerId").Value), SelectedRooms, dtpStartDate, dtpEndDate);
            if (!isSuccess)
            {
                ModelState.AddModelError(string.Empty, "Booking failed. Please try again.");

                // Reload the rooms data to ensure the table is populated
                Rooms = await _customerViewModel.GetAvailableRooms();

                return Page();
            }

            return RedirectToPage("/Index");
        }

        private bool IsValidData()
        {
            if (dtpStartDate >= dtpEndDate)
            {
                ModelState.AddModelError("dtpEndDate", "End date must be after start date.");
                return false;
            }

            if (dtpStartDate < DateTime.Now.Date || dtpEndDate < DateTime.Now.Date)
            {
                ModelState.AddModelError("dtpStartDate", "Start date cannot be in the past.");
                return false;
            }

            if (SelectedRooms == null || SelectedRooms.Count == 0)
            {
                ModelState.AddModelError("SelectedRooms", "At least one room must be selected.");
                return false;
            }
            return true;
        }
    }
}
