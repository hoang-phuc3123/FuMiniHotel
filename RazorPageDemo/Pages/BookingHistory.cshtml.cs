using DataModel.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.SignalR;
using RazorPage;
using RazorPage.Hubs;
using System.Security.Claims;
using System.Text;
using ViewModel;

namespace RazorPageDemo.Pages
{
    [Authorize(Roles = "Customer")]
    public class BookingHistoryModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;

        private readonly IHubContext<SignalRServer> _signalRHub;

        private readonly CustomerViewModel _customerViewModel;

        private readonly FuminiHotelManagementContext _context;
        private readonly IServiceProvider _serviceProvider;

        public string Message;

        [BindProperty]
        // public List<BookingDetailViewModel> BookingHistory { get; set; }
        public string PaginationHtml { get; set; }
        public PaginatedList<BookingDetailViewModel> BookingHistory { get; set; }
        [BindProperty]
        public int BookingReservationId { get; set; }
        public int PageIndex { get; set; }

        public BookingHistoryModel(CustomerViewModel customerViewModel, ILogger<IndexModel> logger,
            FuminiHotelManagementContext context, IHubContext<SignalRServer> signalRHub,
            IServiceProvider serviceProvider)
        {
            _customerViewModel = customerViewModel;
            _logger = logger;
            _context = context;
            _signalRHub = signalRHub;
            _serviceProvider = serviceProvider;
        }

        public async Task<IActionResult> OnGetAsync(int? pageIndex)
        {
            string? customerId = User.FindFirst("CustomerId").Value;
            if (customerId != null)
            {
                var BookingHistoryList = await _customerViewModel.GetListBookingHistoryAsync(customerId);

                int pageSize = 5;
                BookingHistory = await PaginatedList<BookingDetailViewModel>.CreateAsync(BookingHistoryList, pageIndex ?? 1, pageSize);
                PaginationHtml = GeneratePaginationHtml();

            }
            return Page();
        }

        [HttpPost]
        public async Task<IActionResult> OnPostAsync()
        {
            var bookingReservation = await _customerViewModel.GetAsync(BookingReservationId);
            if (bookingReservation == null)
            {
                Message = "Booking Reservation not found.";
                return Page();
            }

            List<BookingDetail> bookingDetails = await _customerViewModel
                .GetAllBookingDetailByReservation(bookingReservation.BookingReservationId);

            _customerViewModel.DeleteRangeBookingDetail(bookingDetails);
            _customerViewModel.DeleteBookingReservation(bookingReservation);
            _customerViewModel.SaveChange();

            OnGetAsync(null);

            //string? customerId = User.FindFirst("CustomerId").Value;
            //if (customerId != null)
            //{
            //    var BookingHistoryList = await _customerViewModel.GetListBookingHistoryAsync(customerId);

            //    int pageSize = 5;
            //    BookingHistory = await PaginatedList<BookingDetailViewModel>.CreateAsync(BookingHistoryList, 1, pageSize);
            //    PaginationHtml = GeneratePaginationHtml();

            //}
            await _signalRHub.Clients.All.SendAsync("LoadBooking");

            return Page();
        }

        private string GeneratePaginationHtml()
        {
            int currentPage = BookingHistory.PageIndex;
            int totalPages = BookingHistory.TotalPages;
            int startPage = Math.Max(1, currentPage - 3);
            int endPage = Math.Min(totalPages, currentPage + 3);

            var sb = new StringBuilder();


            if (BookingHistory.HasPreviousPage)
            {
                sb.Append("<li class=\"page-item\"><a class=\"page-link pagination-box\" href=\"?pageIndex=" + (currentPage - 1) + "\">Previous</a></li>");
            }
            else
            {
                sb.Append("<li class=\"page-item disabled\"><span class=\"page-link pagination-box\">Previous</span></li>");
            }


            for (int i = startPage; i <= endPage; i++)
            {
                sb.Append($"<li class=\"page-item {(i == currentPage ? "active" : "")}\">");
                sb.Append($"<a class=\"page-link pagination-box\" style=\"margin:5px;\" =  href=\"?pageIndex={i}\">{i}</a>");
                sb.Append($"<a class=\"page-link\" href=\"?pageIndex={i}\" style=\"margin: 5px; background-color: blue; color: white; border-radius: 5px;\">{i}</a>");
                sb.Append("</li>");
            }


            if (BookingHistory.HasNextPage)
            {
                sb.Append("<li class=\"page-item\"><a class=\"page-link pagination-box\" href=\"?pageIndex=" + (currentPage + 1) + "\">Next</a></li>");
            }
            else
            {
                sb.Append("<li class=\"page-item disabled\"><span class=\"page-link pagination-box\">Next</span></li>");
            }

            return sb.ToString();
        }
    }
}