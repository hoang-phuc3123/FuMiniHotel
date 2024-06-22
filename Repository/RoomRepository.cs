using DataModel.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository
{
    public class RoomRepository : Repository<RoomInformation>
    {
        public async Task<List<RoomInformation>> GetAvailableRooms(DateTime startDate, DateTime endDate)
        {
            var bookedRoom = await _dbSet
               .Include(r => r.RoomType)
               .Include(r => r.BookingDetails)
               .Where(r => (r.BookingDetails.FirstOrDefault().StartDate >= startDate &&
                           r.BookingDetails.FirstOrDefault().StartDate <= endDate) ||
                           (r.BookingDetails.FirstOrDefault().EndDate >= startDate &&
                           r.BookingDetails.FirstOrDefault().EndDate <= endDate))
               .ToListAsync();

            var allRooms = await _dbSet
                .Include(r => r.RoomType)
                .Include(r => r.BookingDetails)
                .ToListAsync();

            var availableRooms = allRooms
                .Where(r => !bookedRoom.Contains(r))
                .ToList();

            return availableRooms;
        }

        public async Task<RoomInformation?> GetRoomByRoomNumber(string roomNumber)
        {
            return await _dbSet
                .Include(r => r.RoomType)
                .FirstOrDefaultAsync(r => r.RoomNumber == roomNumber);
        }
        public RoomInformation? GetRoomById(int id)
        {
            return _dbSet.Include(r => r.RoomType).FirstOrDefault(r => r.RoomId == id);
        }
        public async Task<bool> UpdateRoomStatus(int id, byte status)
        {
            RoomInformation? room = this.GetRoomById(id);
            if (room == null)
            {
                return false;
            }
            room.RoomStatus = status;
            try
            {
                await this.UpdateAsync(room);
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }
    }
}
