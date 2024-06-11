using DataModel.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository
{
    public class CustomerRepository : Repository<Customer> 
    {
        //Get Customer by email and check password
        public Customer? GetCustomerByEmailAndPassword(string email, string password)
        {
            return this.GetAllAsync().Result.FirstOrDefault(x => x.EmailAddress == email && x.Password == password);
        }
        public Customer? GetCustomerById(int id)
        {
            return this.GetAllAsync().Result.FirstOrDefault(x => x.CustomerId == id);
        }
    }
}
