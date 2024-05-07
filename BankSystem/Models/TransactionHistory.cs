using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BankSystem.Models
{
    public class TransactionHistory
    {
        public int AccountId { get; set; }
        public int  Id{ get; set; }
        public DateTime TransactionDate { get; set; }
        [Required(ErrorMessage = "Amount is required.")]
        public decimal Amount { get; set; }

        [Required(ErrorMessage = "Description is required.")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Please Select Transaction Type.")]
        public string TransactionType { get; set; }
        public int? RecipientAccountId { get; set; }


    }
}
