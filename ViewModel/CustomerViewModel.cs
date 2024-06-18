using DataModel.Models;
using Microsoft.EntityFrameworkCore;
using Repository;
using System.ComponentModel.DataAnnotations;

namespace ViewModel
{
    public class CustomerViewModel
    {
        private readonly CustomerRepository _customerRepository;
        private readonly BookingReservationRepository _bookingReservationRepository;
        private readonly RoomRepository _roomRepository;
        private readonly BookingDetailRepository _bookingDetailRepository;

        public CustomerViewModel()
        {
            _bookingReservationRepository = new BookingReservationRepository();
            _customerRepository = new CustomerRepository();
            _roomRepository = new RoomRepository();
            _bookingDetailRepository = new BookingDetailRepository();
        }

        public async Task<IEnumerable<Customer>> GetAllCustomersAsync()
        {
            return await _customerRepository.GetAllAsync();
        }

        public async Task<UserViewModel> GetCustomerByIdAsync(object id)
        {
            var user = await _customerRepository.GetByIdAsync(id);
            return new UserViewModel
            {
                Id = user.CustomerId,
                Name = user.CustomerFullName,
                Email = user.EmailAddress
            };
        }

        
        public UserViewModel GetCustomerByEmailAndPassword(string email, string password)
        {
            Customer? customer = _customerRepository.GetCustomerByEmailAndPassword(email, password);
            if (customer == null)
            {
                return null;
            }
            else
            {
                return new UserViewModel
                {
                    Id = customer.CustomerId,
                    Name = customer.CustomerFullName,
                    Email = customer.EmailAddress
                };
            }
        }
        public async Task<List<BookingDetailViewModel>> GetListBookingHistoryAsync(String id)
        { 
            int customerId = Int32.Parse(id);
            var bookingHistory = await _bookingReservationRepository.GetBookingHistoryByCustomerIdAsync(customerId);
            var bookingDetails = bookingHistory
                .SelectMany(br => br.BookingDetails.Select(bd => new BookingDetailViewModel
                {
                    BookingReservationID = br.BookingReservationId,
                    BookingDate = br.BookingDate,
                    TotalPrice = br.TotalPrice,
                    BookingStatus = br.BookingStatus,
                    RoomNumber = bd.Room.RoomNumber,
                    StartDate = bd.StartDate,
                    EndDate = bd.EndDate,
                    ActualPrice = bd.ActualPrice
                }))
                .ToList();
            return bookingDetails;
        }
        public async Task<List<RoomViewModel>> GetAvailableRooms(DateTime startDate, DateTime endDate)
        {
            //List<RoomViewModel> rooms = new List<RoomViewModel>();  
            //List<BookingDetail> bookingDetails = (List<BookingDetail>)await _bookingDetailRepository.GetAllAsync();  
            var availableRooms = await _roomRepository.GetAvailableRooms(startDate, endDate);
            var roomViewModels = availableRooms.Select(r => new RoomViewModel
            {
                RoomId = r.RoomId,
                RoomNumber = r.RoomNumber,
                RoomDetailDescription = r.RoomDetailDescription,
                RoomMaxCapacity = r.RoomMaxCapacity,
                RoomTypeId = r.RoomTypeId,
                RoomPricePerDay = r.RoomPricePerDay,
                RoomTypeName = r.RoomType.RoomTypeName
            }).ToList();

            return roomViewModels;
        }
        public async Task<bool>BookRooms(int CustomerId, List<string> RoomNumbers, DateTime startDate, DateTime endDate)
        {
            var customer = _customerRepository.GetCustomerById(CustomerId);
            if (customer == null)
            {
                return false;
            }
            if (startDate.Date >= endDate.Date)
            {
                return false;
            }
            if (startDate.Date < DateTime.Now.Date)
            {
                return false;
            }

            var rooms = new List<RoomInformation>();
            foreach (var roomNumber in RoomNumbers)
            {
                var room = await _roomRepository.GetRoomByRoomNumber(roomNumber);
                if (room == null )
                {
                    return false;
                }
                rooms.Add(room);
            }

            TimeSpan duration = endDate - startDate;
            int numberOfDays = duration.Days;

            var bookingReservation = new BookingReservation
            {
                BookingReservationId = GenerateRandomInt(1, 1000000),
                BookingDate = DateTime.Now,
                TotalPrice = rooms.Sum(r => r.RoomPricePerDay) * numberOfDays,
                CustomerId = customer.CustomerId,
                BookingStatus = 1
            };

            try
            {
                await _bookingReservationRepository.AddAsync(bookingReservation);

                foreach (var room in rooms)
                {
                    var bookingDetail = new BookingDetail
                    {
                        StartDate = startDate,
                        EndDate = endDate,
                        ActualPrice = room.RoomPricePerDay,
                        RoomId = room.RoomId,
                        BookingReservationId = bookingReservation.BookingReservationId
                    };
                    await _bookingDetailRepository.AddAsync(bookingDetail);
                    //_roomRepository.UpdateRoomStatus(room.RoomId, 0);
                }
            }
            catch (Exception e)
            {
                return false;
            }
            return true;
        }
        private int GenerateRandomInt(int min, int max)
        {
            Random random = new Random();
            return random.Next(min, max);
        }
        public Task<CustomerProfile> GetCustomerProfileAsync(int id)
        {
            Customer customer = _customerRepository.GetCustomerById(id);
            return Task.FromResult(new CustomerProfile
            {
                CustomerId = customer.CustomerId,
                FullName = customer.CustomerFullName,
                Phone = customer.Telephone,
                Email = customer.EmailAddress,
                BirthDay = customer.CustomerBirthday
            });
        }
        public async Task<bool> UpdateCustomerProfileAsync(CustomerProfile customer)
        {
            // Implement your logic to update the customer profile
            var existingCustomer = _customerRepository.GetCustomerById(customer.CustomerId);

            if (existingCustomer == null)
            {
                return false;
            }

            existingCustomer.CustomerFullName = customer.FullName;
            existingCustomer.Telephone = customer.Phone;
            existingCustomer.EmailAddress = customer.Email;
            existingCustomer.CustomerBirthday = customer.BirthDay;

            
            await _customerRepository.UpdateAsync(existingCustomer);

            return true;
        }

        public async Task<BookingReservation> GetAsync(int bookingReserationId)
        {
            return await _bookingReservationRepository.GetByIdAsync(bookingReserationId);
        }

        public async Task<List<BookingDetail>> GetAllBookingDetailByReservation(int bookingReserationId)
        {
            var bookingDetails =  await _bookingDetailRepository.GetAllAsync();
            List<BookingDetail> result = bookingDetails.Where(b => b.BookingReservationId == bookingReserationId)
                .ToList();
            return result;
        }

        public async void DeleteBookingReservation(BookingReservation bookingReseration)
        {
            _bookingReservationRepository.HardDelete(bookingReseration);
        }

        public void DeleteRangeBookingDetail(List<BookingDetail> bookingDetails)
        {
                _bookingDetailRepository.HardDeleteRange(bookingDetails);
        }

        public async Task SaveChange()
        {
            await _bookingReservationRepository.SaveChange();
        }

    }
    public class BookingDetailViewModel
    {
        public int BookingReservationID { get; set; }
        public DateTime? BookingDate { get; set; }
        public decimal? TotalPrice { get; set; }
        public byte? BookingStatus { get; set; }
        public string RoomNumber { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public decimal? ActualPrice { get; set; }
    }
    public class RoomViewModel
    {
        public int RoomId { get; set; }
        public string RoomNumber { get; set; }
        public string RoomDetailDescription { get; set; }
        public int? RoomMaxCapacity { get; set; }
        public int RoomTypeId { get; set; }
        public decimal? RoomPricePerDay { get; set; }
        public string RoomTypeName { get; set; }
        public bool Selected { get; set; }
        
    }
    public class CustomerProfile
    {
        public int CustomerId { get; set; }

        [Required]
        [Display(Name = "Full Name")]
        public string FullName { get; set; }

        [Required]
        [RegularExpression(@"^\d{10}$", ErrorMessage = "Phone number must be 10 digits")]
        [Display(Name = "Telephone")]
        public string Phone { get; set; }

        [Required]
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Birthday is required")]
        [DataType(DataType.Date)]
        [Display(Name = "Birthday")]
        [ValidateBirthDate(ErrorMessage = "You must be at least 16 years old")]
        public DateTime? BirthDay { get; set; }
    }
    public class ValidateBirthDateAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var birthDate = (DateTime)value;
            var age = DateTime.Today.Year - birthDate.Year;
            if (age < 16)
            {
                return new ValidationResult(ErrorMessage);
            }
            return ValidationResult.Success;
        }
    }

}