using DataModel.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.SignalR;
using RazorPage.Hubs;
using System.ComponentModel.DataAnnotations;
using ViewModel;

namespace RazorPage.Pages
{
    public class BookingModel : PageModel
    {
        private readonly CustomerViewModel _customerViewModel;
        private readonly IHubContext<SignalRServer> _signalRHub;

        [BindProperty]
        public List<RoomViewModel> Rooms { get; set; }

        [BindProperty]
        public DateTime dtpStartDate { get; set; }

        [BindProperty]
        public DateTime dtpEndDate { get; set; }

        [BindProperty(SupportsGet = true)]
        public List<string> SelectedRooms { get; set; }

        public BookingModel(CustomerViewModel customerViewModel, IHubContext<SignalRServer> signalRHub)
        {
            _customerViewModel = customerViewModel;
            dtpStartDate = DateTime.Today;
            dtpEndDate = DateTime.Today.AddDays(1);
            _signalRHub = signalRHub;
        }

        public async Task OnGetAsync()
        {
            Rooms = await _customerViewModel.GetAvailableRooms(dtpStartDate, dtpEndDate);
        }

        [HttpPost]
        public async Task<IActionResult> OnPostAsync(string action)
        {
            switch (action)
            {
                case "validate":
                    return await GetRoom();
                case "book":
                    return await Book();
                default:
                    return Page();
            }
        }

        public async Task<IActionResult> Book()
        {
            if (!IsValidData())
            {
                //ModelState.AddModelError(string.Empty, "Invalid data. Please check your entries.");

                //Rooms = await _customerViewModel.GetAvailableRooms(dtpStartDate, dtpEndDate);

                return Page();
            }

            var isSuccess = await _customerViewModel.BookRooms(Int32.Parse(User.FindFirst("CustomerId").Value), SelectedRooms, dtpStartDate, dtpEndDate);
            await _signalRHub.Clients.All.SendAsync("LoadBooking");
            if (!isSuccess)
            {
                ModelState.AddModelError(string.Empty, "Booking failed. Please try again.");

                Rooms = await _customerViewModel.GetAvailableRooms(dtpStartDate, dtpEndDate);

                return Page();
            }
            
            return RedirectToPage("/Index");
        }

        public async Task<IActionResult> GetRoom()
        {
            if(dtpStartDate >= dtpEndDate)
            {
                ModelState.AddModelError(string.Empty, "StartDate must be less than EndDate");
            }
            else
            {
                ModelState.Clear();
                Rooms = await _customerViewModel.GetAvailableRooms(dtpStartDate, dtpEndDate);
            }
            return Page();
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
