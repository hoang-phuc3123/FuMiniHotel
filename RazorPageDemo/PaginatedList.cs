using Microsoft.EntityFrameworkCore;
using System.Text;

namespace RazorPage
{
    public class PaginatedList<T>: List<T>
    {
        public int PageIndex { get; private set; }
        public int TotalPages { get; private set; }

        public PaginatedList(List<T> items, int count, int pageIndex, int pageSize)
        {
            PageIndex = pageIndex;
            TotalPages = (int)Math.Ceiling(count / (double)pageSize);

            this.AddRange(items);
        }

        public bool HasPreviousPage
        {
            get
            {
                return (PageIndex > 1);
            }
        }

        public bool HasNextPage
        {
            get
            {
                return (PageIndex < TotalPages);
            }
        }

        public static async Task<PaginatedList<T>> CreateAsync(
            IEnumerable<T> source, int pageIndex, int pageSize)
        {
            var count = source.Count();
            var items = source.Skip(
                (pageIndex - 1) * pageSize)
                .Take(pageSize).ToList();
            return new PaginatedList<T>(items, count, pageIndex, pageSize);
        }
        public string GeneratePaginationHtml()
        {
            StringBuilder sb = new StringBuilder();
            int startPage, endPage;

            sb.Append($"<li><a href=\"?pageIndex=1\" class=\"{(PageIndex == 1 ? "active" : "")}\">1</a></li>");

            if (TotalPages <= 9)
            {
                startPage = 2;
                endPage = TotalPages - 1;
            }
            else
            {
                if (PageIndex <= 5)
                {
                    startPage = 2;
                    endPage = 7;
                }
                else if (PageIndex >= TotalPages - 4)
                {
                    startPage = TotalPages - 6;
                    endPage = TotalPages - 1;
                }
                else
                {
                    startPage = PageIndex - 3;
                    endPage = PageIndex + 3;
                }
            }

            if (startPage > 2)
            {
                sb.Append("<li><span>...</span></li>");
            }

            for (int i = startPage; i <= endPage; i++)
            {
                sb.Append($"<li><a href=\"?pageIndex={i}\" class=\"{(i == PageIndex ? "active" : "")}\">{i}</a></li>");

            }

            if (endPage < TotalPages - 1)
            {
                sb.Append("<li><span>...</span></li>");
            }

            if (TotalPages > 1)
            {
                sb.Append($"<li><a href=\"?pageIndex={TotalPages}\" class=\"{(PageIndex == TotalPages ? "active" : "")}\">{TotalPages}</a></li>");
            }

            return sb.ToString();
        }
    }
}
