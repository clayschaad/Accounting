using System;
using Microsoft.AspNetCore.Mvc;
using Schaad.Accounting.Interfaces;

namespace Schaad.Accounting.Controllers
{
    public class ReportController : BaseController
    {
        private readonly ISettingsService settingsService;
        private readonly IViewService viewService;
        private readonly IFileService fileService;

        public ReportController(IViewService viewService, ISettingsService settingsService, IFileService fileService)
        {
            this.viewService = viewService;
            this.settingsService = settingsService;
            this.fileService = fileService;
        }

        public IActionResult ProfitLoss()
        {
            SetViewDataTitleAndFooter("Erfolgsrechnung");
            var model = viewService.GetAccountViewList();
            return View(model);
        }

        public IActionResult Detail()
        {
            SetViewDataTitleAndFooter("Detailaufstellung");
            var model = viewService.GetAccountViewList();
            ViewBag.TrxList = viewService.GetTransactionViewList();
            return View(model);
        }

        public IActionResult Balance()
        {
            SetViewDataTitleAndFooter("Bilanz");
            var model = viewService.GetBalanceView();
            return View(model);
        }

        public IActionResult BalanceSheet()
        {
            SetViewDataTitleAndFooter("Jahresabschluss");
            var model = viewService.GetBalanceSheetView(settingsService.GetYear());
            return View(model);
        }

        public IActionResult AccountExport(string id)
        {
            var fileBytes = fileService.GetTransactionListCsv(id);
            return File(fileBytes, "text/comma-separated-values", $"{id}.csv");
        }

        private void SetViewDataTitleAndFooter(string title)
        {
            ViewData["Title"] = $"{title} {settingsService.GetMandator()} {settingsService.GetYear()}";

            var footer = "Stand: ";
            if (DateTime.Now.Year == settingsService.GetYear())
            {
                footer += DateTime.Now.ToString("dd.MM.yyyy");
            }
            else
            {
                footer += "31.12." + settingsService.GetYear();
            }
            ViewData["Footer"] = footer;
        }
    }
}