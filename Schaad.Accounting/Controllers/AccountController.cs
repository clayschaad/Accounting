using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Schaad.Accounting.Interfaces;
using Schaad.Accounting.Models;

// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace Schaad.Accounting.Controllers
{
    public class AccountController : BaseController
    {
        private readonly IAccountRepository accountRepository;
        private readonly IViewService viewService;

        public AccountController(IViewService viewService, IAccountRepository accountRepository)
        {
            this.viewService = viewService;
            this.accountRepository = accountRepository;
        }

        public IActionResult Index()
        {
            var model = accountRepository.GetAccountList();
            ViewData["Title"] = "Konten";
            return View(model);
        }

        public IActionResult GetAccountList()
        {
            var model = accountRepository.GetAccountList().OrderBy(a => a.Number);
            return Json(model.Select(a => new {id = a.Id, text = a.Number + " / " + a.Name}));
        }

        public IActionResult GetTransactionList(string id)
        {
            var transactions = viewService.GetTransactionViewList(id);
            ViewBag.Account = viewService.GetAccountView(id);
            ViewBag.AccountId = id;
            return View("PartialList", transactions);
        }

        public bool IsFxAccount(string id)
        {
            var account = accountRepository.GetAccount(id);
            return account.IsFxAccount;
        }

        public IActionResult Edit(string id)
        {
            ViewBag.ReturnUrl = "/Account";
            if (string.IsNullOrEmpty(id))
            {
                return View(new Account());
            }
            else
            {
                var account = accountRepository.GetAccount(id);
                return View(account);
            }
        }

        [HttpPost]
        public JsonResult Save([FromBody] Account account)
        {
            //if (ModelState.IsValid)
            {
                accountRepository.SaveAccount(account);      
            }

            return new JsonResult("");
        }
    }
}