using BankSystem.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Threading.Tasks;

namespace BankSystem
{
    public class BankDbContext : DbContext
    {

        public BankDbContext() : base("Server=.\\sqlexpress;Initial Catalog=BankSystem;Integrated Security=true;")
        {
          
        }
        public DbSet<Bank> Banks { get; set; }

        public DbSet<Customer> Customers { get; set; }
        public DbSet<Account> Accounts { get; set; }

        public DbSet<TransactionHistory> TransactionHistories { get; set; }
    }
}
