using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Net.Http.Headers;
using Schaad.Accounting.Datasets;
using Schaad.Accounting.Interfaces;
using Schaad.Accounting.Models;

namespace Schaad.Accounting.Controllers
{
    public class HomeController : BaseController
    {
        private readonly IAccountRepository accountRepository;
        private readonly IBankTransactionRepository bankTransactionRepository;
        private readonly IBookingRuleRepository bookingRuleRepository;
        private readonly IFileService fileService;
        private readonly ILogger logger;
        private readonly ISettingsService settingsService;
        private readonly ISplitPredefinitonRepository splitPredefinitonRepository;
        private readonly ITransactionRepository transactionRepository;
        private readonly IViewService viewService;

        public HomeController(
            ILoggerFactory loggerFactory,
            IViewService viewService,
            ISettingsService settingsService,
            IFileService fileService,
            IAccountRepository accountRepository,
            ITransactionRepository transactionRepository,
            IBankTransactionRepository bankTransactionRepository,
            IBookingRuleRepository bookingRuleRepository,
            ISplitPredefinitonRepository splitPredefinitonRepository)
        {
            this.logger = loggerFactory.CreateLogger(nameof(HomeController));
            this.viewService = viewService;
            this.settingsService = settingsService;
            this.fileService = fileService;
            this.accountRepository = accountRepository;
            this.transactionRepository = transactionRepository;
            this.bankTransactionRepository = bankTransactionRepository;
            this.bookingRuleRepository = bookingRuleRepository;
            this.splitPredefinitonRepository = splitPredefinitonRepository;
        }

        public IActionResult Index()
        {
            logger.LogInformation("Start");
            SetSession("Mandator", settingsService.GetMandator());
            SetSession("Year", settingsService.GetYear().ToString());

            var model = viewService.GetOpenBankTransactionList();
            return View("Index", model);
        }

        public IActionResult ChangeYear(int id)
        {
            settingsService.SetYear(id);
            SetSession("Year", id.ToString());
            return LocalRedirect("/");
        }

        public IActionResult ChangeMandator(string id)
        {
            settingsService.SetMandator(id);
            SetSession("Mandator", id);
            return LocalRedirect("/");
        }

        public IActionResult Backup()
        {
            var result = fileService.Backup();
            ViewBag.Messages = new List<MessageDataset> {new MessageDataset(result)};
            return Index();
        }

        [HttpPost]
        public async Task<IActionResult> Upload(ICollection<IFormFile> files)
        {
            var messageList = new List<MessageDataset>();
            var uploads = settingsService.GetUploadPath();
            foreach (var file in files)
            {
                if (file.Length > 0)
                {
                    var fileName = DateTime.Now.ToString("yyyyMMdd") + "_" +
                                   Path.GetFileName(ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.ToString().Trim('"'));
                    var serverFileName = Path.Combine(uploads, fileName);
                    using (var fs = new FileStream(serverFileName, FileMode.Create))
                    {
                        await file.CopyToAsync(fs);
                    }
                    var messages = fileService.ImportAccountStatementFile(serverFileName);
                    messageList.AddRange(messages);
                }
            }

            ViewBag.Messages = messageList;
            return Index();
        }

        /// <summary>
        /// Edit open bank transaction
        /// </summary>
        /// <returns></returns>
        public IActionResult Edit()
        {
            var transactionList = viewService.MatchOpenBankTransactions();
            return View(transactionList);
        }

        /// <summary>
        /// Save a transction
        /// </summary>
        /// <param name="transaction"></param>
        [HttpPost]
        public JsonResult SaveTransaction([FromBody] Transaction transaction)
        {
            transactionRepository.SaveTransaction(transaction);
            return new JsonResult("");
        }

        /// <summary>
        /// Show bank transaction split dialog
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public IActionResult Split(string id)
        {
            var accounts = accountRepository.GetAccountList();
            var bankTrx = bankTransactionRepository.GetBankTransaction(id);
            var trx = new Transaction(bankTrx, accounts);
            return View("Split", trx);
        }

        public IActionResult GetEmptyTransaction(string id)
        {
            var accounts = accountRepository.GetAccountList();
            var bankTrx = bankTransactionRepository.GetBankTransaction(id);
            var trx = new Transaction(bankTrx, accounts);
            return PartialView("Transaction", trx);
        }

        public IActionResult GetSplitPredefiniton(string id)
        {
            var accounts = accountRepository.GetAccountList();
            var bankTrx = bankTransactionRepository.GetBankTransaction(id);
            var splitDefinitionList = splitPredefinitonRepository.GetSplitPredefinitionList();
            var trxList = splitDefinitionList.Select(
                    s => new Transaction(bankTrx, accounts) {Value = -s.BookingValue, Text = s.BookingText, TargetAccountId = s.AccountId})
                .ToList();
            var res = PartialView("TransactionList", trxList);
            return res;
        }

        [HttpPost]
        public JsonResult SaveSplitTransaction([FromBody] List<Transaction> transactions)
        {
            foreach (var transaction in transactions)
            {
                transactionRepository.SaveTransaction(transaction);
            }
            return new JsonResult("");
        }

        public IActionResult Error()
        {
            return View();
        }
    }
}