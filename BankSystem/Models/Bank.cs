using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BankSystem.Models
{
    public class Bank
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public List<Account> Accounts { get; set; }

        public List<Customer> Customers { get; set; }
    }
}
