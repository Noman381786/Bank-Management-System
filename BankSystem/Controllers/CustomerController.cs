using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using BankSystem.Models;
using Microsoft.AspNetCore.Http;
using System.Net.Mail;
using System.Net;
using Microsoft.Extensions.Configuration;

namespace BankSystem.Controllers
{
    public class CustomerController : Controller
    {
        BankDbContext _context = new BankDbContext();
        public IActionResult Index()
        {
            var customers = _context.Customers.ToList();
            return View(customers);
        }
        public IActionResult Signin()
        {
            return View();
        }

        [HttpPost]
        public ActionResult SignIn(Customer login)
        {
            var customer=_context.Customers
                    .FirstOrDefault(c => c.Email == login.Email);
            // Check if the credentials are valid (this is just a placeholder)
            if (customer != null && VerifyPassword(login.Password, customer.Password))
            {
                HttpContext.Session.SetInt32("CustomerId", customer.Id);
                // Redirect to a dashboard or home page after successful sign-in
                return RedirectToAction("Transactions", "Customer");
            }
            else
            {
                ViewBag.Error = "Invalid email or password";
                return View(login);
            }
        }
    
    [HttpGet]
        public IActionResult CreateCustomer()
        {
            return View();
        }
        [HttpPost]
        public IActionResult CreateCustomer(Customer newCustomer, string AccountType)
        {
            newCustomer.BankId = 1;
            newCustomer.CreatedOn = DateTime.Now;
            newCustomer.Password = EncodePasswordToBase64(newCustomer.Password);
            _context.Customers.Add(newCustomer);
            _context.SaveChanges();
            int maxAccount = _context.Accounts.Max(a => a.AccountNumber);
            if (maxAccount==0)
            {
                maxAccount = 10001;
            }
            else
            {
                maxAccount++;
            }
            var newAccount = new Account
            {
                AccountType = AccountType == "Personal" ? Models.Enums.AccountType.Personal
                                            : Models.Enums.AccountType.Business,
                BankId = 1,
                CreatedOn = DateTime.Now,
                AccountNumber = maxAccount,
                CustomerId = newCustomer.Id,
                IsActive=true
                
            };
            
            _context.Accounts.Add(newAccount);
            _context.SaveChanges();
            return Redirect("SignIn");
        }
        //private string Encrypt(string plainText)
        //{
        //    using (Aes aesAlg = Aes.Create())
        //    {
        //        // Key and IV should be securely stored and should not be hardcoded here
        //        // For demonstration purposes, I'm using hardcoded values.
        //        aesAlg.Key = Convert.FromBase64String("abyuy642jckaft");
        //        aesAlg.IV = Convert.FromBase64String("BHrtu3n8a1d");

        //        ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

        //        byte[] encryptedBytes = null;
        //        using (var msEncrypt = new System.IO.MemoryStream())
        //        {
        //            using (var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
        //            {
        //                using (var swEncrypt = new System.IO.StreamWriter(csEncrypt))
        //                {
        //                    swEncrypt.Write(plainText);
        //                }
        //                encryptedBytes = msEncrypt.ToArray();
        //            }
        //        }
        //        return Convert.ToBase64String(encryptedBytes);
        //    }
        //}

        // Decryption method
        //private string Decrypt(string cipherText)
        //{
        //    using (Aes aesAlg = Aes.Create())
        //    {
        //        // Key and IV should be securely stored and should not be hardcoded here
        //        // For demonstration purposes, I'm using hardcoded values.
        //        aesAlg.Key = Convert.FromBase64String("YourSecretKeyHere");
        //        aesAlg.IV = Convert.FromBase64String("YourSecretIVHere");

        //        ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

        //        using (var msDecrypt = new System.IO.MemoryStream(Convert.FromBase64String(cipherText)))
        //        {
        //            using (var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
        //            {
        //                using (var srDecrypt = new System.IO.StreamReader(csDecrypt))
        //                {
        //                    return srDecrypt.ReadToEnd();
        //                }
        //            }
        //        }
        //    }
        //}

        // Password verification method
        private bool VerifyPassword(string inputPassword, string storedPassword)
        {
            string decryptedPassword = DecodeFrom64(storedPassword);
            return inputPassword == decryptedPassword;
        }
        //this function Convert to Encord your Password
        public static string EncodePasswordToBase64(string password)
        {
            try
            {
                byte[] encData_byte = new byte[password.Length];
                encData_byte = System.Text.Encoding.UTF8.GetBytes(password);
                string encodedData = Convert.ToBase64String(encData_byte);
                return encodedData;
            }
            catch (Exception ex)
            {
                throw new Exception("Error in base64Encode" + ex.Message);
            }
        }
        //this function Convert to Decord your Password
        public string DecodeFrom64(string encodedData)
        {
            System.Text.UTF8Encoding encoder = new System.Text.UTF8Encoding();
            System.Text.Decoder utf8Decode = encoder.GetDecoder();
            byte[] todecode_byte = Convert.FromBase64String(encodedData);
            int charCount = utf8Decode.GetCharCount(todecode_byte, 0, todecode_byte.Length);
            char[] decoded_char = new char[charCount];
            utf8Decode.GetChars(todecode_byte, 0, todecode_byte.Length, decoded_char, 0);
            string result = new String(decoded_char);
            return result;
        }
        public IActionResult Transactions(DateTime? startDate, DateTime? endDate,decimal? minimumAmount ,decimal? maximumAmount, string transactionType)
        {
            var customer = Shared.GetLoggedInCustomer(HttpContext);
           
            
            ViewBag.CustomerName = customer.Name;
            ViewBag.CustomerId = customer.Id;
            var account = _context.Accounts.FirstOrDefault(x => x.CustomerId == customer.Id);
            //ViewBag.AccountType = account.AccountType == Models.Enums.AccountType.Personal ? "Personal" : "Business";
            ViewBag.IsActive = account.IsActive;
            ViewBag.AccountType = account.AccountType.ToString();
            ViewBag.AccountId = account.Id;
            var query = _context.TransactionHistories
                                    .Where(x => x.AccountId == account.Id);
            
            // Apply date range filter if provided
            if (startDate.HasValue)
            {
                query = query.Where(x => x.TransactionDate >= startDate );
            }
            if (endDate.HasValue)
            {
                endDate =endDate.Value.AddDays(1);
                query = query.Where(x => x.TransactionDate < endDate);
            }
            if (maximumAmount.HasValue)
            {
                query = query.Where(x => x.Amount <= maximumAmount);
            }
            if (minimumAmount.HasValue)
            {
                query = query.Where(x => x.Amount >= minimumAmount);
            }
            if (!string.IsNullOrWhiteSpace(transactionType))
            {
                query = query.Where(x => x.TransactionType == transactionType);
            }
            var creditAmount = query.Where(x => x.TransactionType == "Credit").Sum(x => (decimal?)x.Amount) ?? 0m;
            var debitAmount = query.Where(x => x.TransactionType == "Debit").Sum(x => (decimal?)x.Amount) ?? 0m;
           var balance = Math.Round((creditAmount - debitAmount), 2);
            ViewBag.AccountBalance = balance;
            var accountTransactions = query.ToList();
            return startDate.HasValue || startDate.HasValue  || !string.IsNullOrWhiteSpace(transactionType) || maximumAmount.HasValue || minimumAmount.HasValue
            ? new JsonResult( new
            {
                Balance = balance,
                ObjectList = accountTransactions
            }) : View(accountTransactions);

        }

        [HttpGet]
        public IActionResult Transfer(int accountId)
        {
            ViewBag.AccountId = accountId;
            return View();
            
        }

        [HttpPost]
        public IActionResult Transfer(decimal Amount, int recepientAccount, int AccountId)
        {
            // search recepient Account
            var toAccount = _context.Accounts.FirstOrDefault(x => x.AccountNumber == recepientAccount);
            if (toAccount == null)
            {
                ViewBag.AccountId = AccountId;

                return Json(new { success = false });
            }

            // transfer 
            var fromAccount = _context.Accounts.First(x => x.Id == AccountId);

            // to do, check balance, cannot transfer more than available balance
            var toAccountTransaction = new TransactionHistory
            {
                AccountId = toAccount.Id,
                Description = "Funds Received",
                Amount = Amount,
                TransactionDate = DateTime.Now,
                TransactionType = "Credit"
            };

            var fromAccountTransaction = new TransactionHistory
            {
                AccountId = fromAccount.Id,
                Description = "Funds Transfered",
                Amount = Amount,
                TransactionDate = DateTime.Now,
                TransactionType = "Debit",
                RecipientAccountId = toAccount.Id
            };

            _context.TransactionHistories.Add(toAccountTransaction);
            _context.TransactionHistories.Add(fromAccountTransaction);
            _context.SaveChanges();
            var returnvalue = fromAccountTransaction;
            returnvalue.RecipientAccountId = fromAccount.AccountNumber;
            return new JsonResult(new
            {
                transactionHistory = returnvalue
            });
            //return RedirectToAction("Transactions");
        }

        [HttpGet]
        public IActionResult CreateTransaction(int accountId)
        {
            ViewBag.AccountId = accountId;
            return View();
        }

        [HttpPost]
        public IActionResult CreateTransaction(TransactionHistory newTransaction)
        {
            if (ModelState.IsValid)
            {
                newTransaction.TransactionDate = DateTime.Now;
                _context.TransactionHistories.Add(newTransaction);
                var account = _context.Accounts.FirstOrDefault(x => x.Id == newTransaction.AccountId);
                if (newTransaction.TransactionType == "Debit")
                {
                    account.Balance -= newTransaction.Amount;
                }
                else
                {
                    account.Balance += newTransaction.Amount;
                }

                _context.SaveChanges();
                return new JsonResult(new
                {
                    date = DateTime.Now,
                    accountNumber = account.AccountNumber
                });
            }
            ViewBag.AccountId = newTransaction.AccountId;
            return View(newTransaction);
        }
        [HttpGet]
        public IActionResult EditCustomerDetails(int customerId)
        {
            // Retrieve the customer from the database
            var customer = _context.Customers.FirstOrDefault(c => c.Id == customerId);
            ViewBag.CustomerId = customer.Id;
            if (customer == null)
            {
                // Handle case where customer is not found
                return NotFound();
            }

            return View();
        }

        [HttpPost]
        public IActionResult EditCustomerDetails(Customer editedCustomer,string AccountType)
        
        {
            // Retrieve the customer from the database
            var existingCustomer = _context.Customers.FirstOrDefault(c => c.Id == editedCustomer.Id);
            var updateAccount = _context.Accounts.FirstOrDefault(c => c.CustomerId == editedCustomer.Id);
            if (existingCustomer == null)
            {
                // Handle case where customer is not found
                return NotFound();
            }
            updateAccount.AccountType = AccountType == "Personal" ? Models.Enums.AccountType.Personal
                                            : Models.Enums.AccountType.Business;
            updateAccount.BankId = 1;
            updateAccount.CreatedOn = DateTime.Now;
        


            // Update the existing customer with the edited details
            existingCustomer.Name = editedCustomer.Name;
            existingCustomer.Email = editedCustomer.Email;
            existingCustomer.Password = DecodeFrom64(editedCustomer.Password);
            existingCustomer.BankId = 1;
            existingCustomer.CreatedOn = DateTime.Now;
            

            // Save changes to the database
            _context.SaveChanges();

            // Redirect to a relevant action, such as the customer details page
            return RedirectToAction("Transactions");
        }

        [HttpPost]
        public IActionResult CardRequestSend()
        {
           // EmailSend("test", "test");
            // Redirect to a relevant action
            return Json(new { success = true });
        }

        [HttpPost]
        public IActionResult CheckBookRequestSend()
        {
         // EmailSend("test", "test");
            // Redirect to a relevant action
            return Json(new { success = true });
        }

        public bool EmailSend(string subject, string message)
        {
            IConfiguration config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();

            // Get email settings from appsettings.json
            string senderEmail = config["EmailSettings:SenderEmail"];
            string senderPassword = config["EmailSettings:SenderPassword"];
            string smtpServer = config["EmailSettings:SmtpServer"];
            int smtpPort = int.Parse(config["EmailSettings:SmtpPort"]);
            bool enableSsl = bool.Parse(config["EmailSettings:EnableSsl"]);

            // Sender's email credentials

                // Recipient's email 
                string recipientEmail = "shehrozabbasi901@gmail.com";

                // Create a new MailMessage instance
                MailMessage mail = new MailMessage(senderEmail, recipientEmail);

                // Set the subject and body of the email
                mail.Subject = subject;
                mail.Body = message;

                // Create SMTP client and specify host and port
                SmtpClient smtpClient = new SmtpClient(smtpServer, smtpPort);

            // Specify credentials (if required by your SMTP server)        // Enable SSL encryption and specify authentication method
            smtpClient.EnableSsl = true;
            smtpClient.UseDefaultCredentials = false;
            smtpClient.Credentials = new NetworkCredential(senderEmail, senderPassword);


            try
            {
                    // Send the email
                   smtpClient.Send(mail);
                    Console.WriteLine("Email sent successfully!");
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Failed to send email: " + ex.Message);
                }
            return true;
        }
        [HttpPost]
        public IActionResult ActivateAccount(int accountId)
        {
            var account = _context.Accounts.FirstOrDefault(x => x.Id == accountId);

            if (account == null)
            {
                // Handle case where account is not found
                return NotFound();
            }

            account.IsActive = true;
            _context.SaveChanges();

            // Redirect to a relevant action
            return Json(new { success = true });
        }

        [HttpPost]
        public IActionResult DeactivateAccount(int accountId)
        {
            var account = _context.Accounts.FirstOrDefault(x => x.Id == accountId);

            if (account == null)
            {
                // Handle case where account is not found
                return NotFound();
            }

            account.IsActive = false;
            _context.SaveChanges();

            // Redirect to a relevant action
            return Json(new { success = true });
        }



    }
}
