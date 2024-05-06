using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BankSystem.Controllers
{
    public class BankController : Controller
    {
        BankDbContext _context = new BankDbContext();

        public IActionResult Index()
        {
            var bank = _context.Banks.FirstOrDefault();
            return View(bank);
        }


    }
}
