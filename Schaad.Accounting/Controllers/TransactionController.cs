using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Schaad.Accounting.Interfaces;
using Schaad.Accounting.Models;

namespace Schaad.Accounting.Controllers
{
    public class TransactionController : BaseController
    {
        private readonly IAccountRepository accountRepository;
        private readonly ITransactionRepository transactionRepository;
        private readonly IViewService viewService;
        private readonly ISettingsService settingsService;

        public TransactionController(IViewService viewService, IAccountRepository accountRepository, ITransactionRepository transactionRepository, ISettingsService settingsService)
        {
            this.viewService = viewService;
            this.transactionRepository = transactionRepository;
            this.accountRepository = accountRepository;
            this.settingsService = settingsService;
        }

        public IActionResult Index()
        {
            var model = viewService.GetAccountViewList();
            return View(model);
        }

        public IActionResult Edit(string id)
        {
            ViewBag.ReturnUrl = "/Transaction";
            if (string.IsNullOrEmpty(id))
            {
                var defaultAccount = accountRepository.GetAccountList().First(a => a.Number == 1000);
                return View(
                    new Transaction()
                    {
                        OriginAccountId = defaultAccount.Id,
                        ValueDate = DateTime.Now.Date
                    });
            }
            else
            {
                var transaction = transactionRepository.GetTransaction(id);
                return View(transaction);
            }
        }


        [HttpPost]
        public JsonResult Save([FromBody] Transaction transaction)
        {
            transactionRepository.SaveTransaction(transaction);
            return new JsonResult("");
        }
    }
}