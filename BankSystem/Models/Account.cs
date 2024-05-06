using BankSystem.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BankSystem.Models
{
    public class Account
    {
        public int Id { get; set; }
        public int CustomerId { get; set; }
        public int BankId { get; set; }
        public AccountType AccountType { get; set; }
        public decimal Balance { get; set; }
        public DateTime CreatedOn { get; set; }
        public int AccountNumber { get; set; }
        public bool IsActive { get; set; }

    }
}
