using Microsoft.AspNetCore.Mvc;
using Schaad.Accounting.Interfaces;

// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace Schaad.Accounting.Controllers
{
    public class BankTransactionController : BaseController
    {
        private readonly IBankTransactionRepository bankTransactionRepository;

        public BankTransactionController(IBankTransactionRepository bankTransactionRepository)
        {
            this.bankTransactionRepository = bankTransactionRepository;
        }

        public IActionResult Index()
        {
            var model = bankTransactionRepository.GetBankTransactionList();
            ViewData["Title"] = "Bank Transaktionen";
            return View(model);
        }
    }
}