using DataModel.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository
{
    public class BookingReservationRepository:Repository<BookingReservation>
    {
        

        public async Task<List<BookingReservation>> GetBookingHistoryByCustomerIdAsync(int customerId)
        {
            return await _dbSet
                .Where(br => br.CustomerId == customerId)
                .Include(br => br.BookingDetails)
                .ThenInclude(bd => bd.Room)
                .ToListAsync();
        }
    }
}
