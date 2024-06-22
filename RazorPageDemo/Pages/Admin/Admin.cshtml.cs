using DataModel.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.SignalR;
using RazorPage.Hubs;
using RazorPageDemo.Pages;
using System.Text;
using ViewModel;

namespace RazorPage.Pages.Admin
{
    [Authorize(Roles = "Admin")]
    public class AdminModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;

        private readonly IHubContext<SignalRServer> _signalRHub;

        private readonly CustomerViewModel _customerViewModel;

        private readonly FuminiHotelManagementContext _context;
        [BindProperty]
        // public List<BookingDetailViewModel> BookingHistory { get; set; }
        public string PaginationHtml { get; set; }
        public PaginatedList<BookingDetailViewModel> BookingHistory { get; set; }
        [BindProperty]
        public int BookingReservationId { get; set; }
        public int PageIndex { get; set; }

        public AdminModel(CustomerViewModel customerViewModel, ILogger<IndexModel> logger,
            FuminiHotelManagementContext context, IHubContext<SignalRServer> signalRHub)
        {
            _customerViewModel = customerViewModel;
            _logger = logger;
            _context = context;
            _signalRHub = signalRHub;
        }
        public async Task OnGetAsync(int? pageIndex)
        {
                var BookingHistoryList = await _customerViewModel.GetAllBookingAsync();

                int pageSize = 5;
                BookingHistory = await PaginatedList<BookingDetailViewModel>.CreateAsync(BookingHistoryList, pageIndex ?? 1, pageSize);
                PaginationHtml = GeneratePaginationHtml();

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
