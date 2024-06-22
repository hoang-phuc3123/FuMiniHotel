using DataModel.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.SignalR;
using RazorPage.Hubs;
using ViewModel;

namespace RazorPage.Pages.Customer
{
    [Authorize(Roles = "Customer")]
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
                    return await GetRoom(dtpStartDate, dtpEndDate);
                case "book":
                    return await Book();
                default:
                    return Page();
            }
        }

        public async Task<IActionResult> Book()
        {
            if (!IsValidData(dtpStartDate, dtpEndDate))
            {
                //ModelState.AddModelError(string.Empty, "Invalid data. Please check your entries.");

                //Rooms = await _customerViewModel.GetAvailableRooms(dtpStartDate, dtpEndDate);

                return Page();
            }
            Rooms = await _customerViewModel.GetAvailableRooms(dtpStartDate, dtpEndDate);

            var check = true;
            foreach(var room in Rooms)
            {
                foreach(var selectedRoom in SelectedRooms)
                {
                    if (room.RoomNumber == selectedRoom)
                    {
                        check = false; break;
                    }
                }
            }
            if (check)
            {
                GetRoom(dtpStartDate, dtpEndDate);
                ModelState.AddModelError("SelectedRooms", "Rooms are booked by someone, please book another rooms.");
                return Page();
            }

            var isSuccess = await _customerViewModel.BookRooms(int.Parse(User.FindFirst("CustomerId").Value), SelectedRooms, dtpStartDate, dtpEndDate);
            await _signalRHub.Clients.All.SendAsync("LoadBooking");
            if (!isSuccess)
            {
                ModelState.AddModelError(string.Empty, "Booking failed. Please try again.");

                Rooms = await _customerViewModel.GetAvailableRooms(dtpStartDate, dtpEndDate);

                return Page();
            }

            return RedirectToPage("/BookingHistory");
        }

        public async Task<IActionResult> GetRoom(DateTime startDate, DateTime endDate)
        {
            if (startDate >= endDate)
            {
                ModelState.AddModelError("dtpEndDate", "End date must be after start date.");
                return Page();
            }

            if (startDate < DateTime.Now.Date || endDate < DateTime.Now.Date)
            {
                ModelState.AddModelError("dtpStartDate", "Start date cannot be in the past.");
                return Page();
            }

            ModelState.Clear();
            Rooms = await _customerViewModel.GetAvailableRooms(startDate, endDate);
            return Page();
        }

        private bool IsValidData(DateTime startDate, DateTime endDate)
        {
            if (startDate >= endDate)
            {
                ModelState.AddModelError("dtpEndDate", "End date must be after start date.");
                return false;
            }

            if (startDate < DateTime.Now.Date || endDate < DateTime.Now.Date)
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
