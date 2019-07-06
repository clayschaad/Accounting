using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Schaad.Accounting.Interfaces;
using Schaad.Accounting.Models;
using Schaad.Finance.Api;

namespace Schaad.Accounting.Controllers
{
    public class TransactionController : BaseController
    {
        private readonly IAccountRepository accountRepository;
        private readonly ITransactionRepository transactionRepository;
        private readonly IViewService viewService;
        private readonly IFxService fxService;
        private readonly ISettingsService settingsService;

        public TransactionController(IViewService viewService, IAccountRepository accountRepository, ITransactionRepository transactionRepository, IFxService fxService, ISettingsService settingsService)
        {
            this.viewService = viewService;
            this.transactionRepository = transactionRepository;
            this.accountRepository = accountRepository;
            this.fxService = fxService;
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

        public string GetFxRate(string originAccountId, string targetAccountId, DateTime date)
        {
            var settings = settingsService.GetSettings();
            var originAccount = accountRepository.GetAccount(originAccountId);
            var targetAccount = accountRepository.GetAccount(targetAccountId);
            var targetAccountCurrency = string.IsNullOrEmpty(targetAccount.Currency) ? "CHF" : targetAccount.Currency;
            var fxRate = fxService.GetFxRate(originAccount.Currency, targetAccountCurrency, date, settings.FixerIoApiKey);
            return fxRate.ToString();
        }
    }
}