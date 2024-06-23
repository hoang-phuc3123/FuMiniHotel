using DataModel.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository
{
    public class CustomerRepository : Repository<Customer> 
    {
        private readonly FuminiHotelManagementContext _dbContext = new FuminiHotelManagementContext();

        //Get Customer by email and check password
        public Customer? GetCustomerByEmailAndPassword(string email, string password)
        {
            return this.GetAllAsync().Result.FirstOrDefault(x => x.EmailAddress == email && x.Password == password);
        }

        public async Task<Customer> GetCustomerByEmail(string email)
        {
            return await _dbContext.Customers.FirstOrDefaultAsync(c => c.EmailAddress == email);
        }

        public List<Customer> GetAllCustomer()
        {
            return _dbContext.Customers.Where(c => c.CustomerStatus == 1).ToList();
        }

        public async Task<List<Customer>> GetRegisterCustomer()
        {
            return await _dbContext.Customers.Where(c => c.CustomerStatus == 0).ToListAsync();
        }
        public Customer? GetCustomerById(int id)
        {
            return this.GetAllAsync().Result.FirstOrDefault(x => x.CustomerId == id);
        }
    }
}
