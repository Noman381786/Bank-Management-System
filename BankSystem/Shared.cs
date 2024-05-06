using BankSystem.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BankSystem
{
    public static class Shared
    {
        public static Customer GetLoggedInCustomer(HttpContext httpContext)
        {
            var customerId = httpContext.Session.GetInt32("CustomerId");

            if (customerId.HasValue)
            {
                using (var ctx = new BankDbContext())
                {
                    return ctx.Customers.FirstOrDefault(x => x.Id == customerId);
                }
            }

            return null;
        }
    }
}
