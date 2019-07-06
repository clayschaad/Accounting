using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Schaad.Accounting.Datasets.Charts;
using Schaad.Accounting.Interfaces;

// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace Schaad.Accounting.Controllers
{
    public class ChartController : BaseController
    {
        private readonly IChartService chartService;
        private readonly ISubclassRepository subclassRepository;
        private readonly IViewService viewService;

        public ChartController(
            IChartService chartService,
            IViewService viewService,
            ISubclassRepository subclassRepository)
        {
            this.chartService = chartService;
            this.viewService = viewService;
            this.subclassRepository = subclassRepository;
        }

        #region Expenses

        public IActionResult Expenses()
        {
            ViewData["Title"] = "Ausgaben";

            var subclassNumber = ClassIds.Expenses * 10;
            var expenses = subclassRepository.GetSubClassList().Where(s => s.Number >= subclassNumber);
            if (expenses.Count() == 1)
            {
                return Redirect("ExpensesDetail/" + expenses.First().Number);
            }
            else
            {
                return View();
            }
        }

        public IActionResult ExpensesDetail(int id)
        {
            ViewData["Title"] = "Ausgaben Detail";
            ViewData["id"] = id;
            return View();
        }

        public JsonResult GetExpensesChart()
        {
            var accounts = viewService.GetAccountViewList().Where(a => a.Class == ClassIds.Expenses);
            var list = new List<PieChartElement>();

            foreach (var grp in accounts.GroupBy(a => a.SubClass).Select(a => new {Key = a.Key, List = a.ToList()}))
            {
                if (grp.List.Sum(g => g.Balance) != 0)
                {
                    list.Add(new PieChartElement(grp.List.First().SubClassName, grp.List.Sum(g => g.Balance), grp.List.First().SubClass.ToString()));
                }
            }
            return new JsonResult(list.OrderBy(l => l.y));
        }

        public JsonResult GetExpensesDetailChart(int subClass)
        {
            var accounts = viewService.GetAccountViewList().Where(a => a.SubClass == subClass);
            var list = new List<PieChartElement>();

            foreach (var grp in accounts)
            {
                if (grp.Balance != 0)
                {
                    list.Add(new PieChartElement(grp.Name, grp.Balance, grp.Id));
                }
            }
            return new JsonResult(list.OrderBy(l => l.y));
        }

        #endregion

        #region ExpensesOverTime

        public IActionResult ExpensesOverTime()
        {
            ViewData["Title"] = "Ausgaben pro Monat";
            return View();
        }

        public JsonResult GetExpensesOverTimeChart()
        {
            var transactions = viewService.GetTransactionViewList().Where(a => a.TargetAccount.Class == ClassIds.Expenses);

            // More than one subclass xx
            if (transactions.Select(t => t.TargetAccount.SubClass).Distinct().Count() > 1)
            {
                var list = chartService.GetSubClassExpensesPerMonth();
                return new JsonResult(list);
            }
            // only one subclass xx (z.B. Mandant Mannenbach)
            else
            {
                var list = chartService.GetAccountExpensesPerMonth();
                return new JsonResult(list);
            }
        }

        #endregion

        #region Assets

        public IActionResult Assets()
        {
            ViewData["Title"] = "Vermögen";
            return View();
        }

        public JsonResult GetAssetsChart()
        {
            var list = new List<PieChartElement>();
            var accounts = viewService.GetAccountViewList().Where(a => a.Class == ClassIds.Activa);
            foreach (var account in accounts)
            {
                //_logger.LogDebug(account.ToString());
                if (account.BalanceCHF > 0)
                {
                    list.Add(new PieChartElement(account.Name, account.BalanceCHF, account.Id));
                }
            }
            return new JsonResult(list.OrderBy(l => l.name));
        }

        #endregion

        #region AccountExpensesPerMonth

        public IActionResult AccountExpensesPerMonth(string id)
        {
            var account = viewService.GetAccountView(id);
            ViewData["AccountId"] = id;
            ViewData["Title"] = $"Ausgaben Konto '{account.Name}' pro Monat";
            ViewData["ChartTitle"] = $"Konto {account.Number} / {account.Name}";
            return View();
        }

        public JsonResult GetAccountExpensesOverTimeChart(string id)
        {
            var series = new List<DataSerie>();
            var dataSerie = chartService.GetAccountExpensesPerMonth(id, DateTime.Now.Year - 1);
            if (dataSerie != null)
            {
                series.Add(dataSerie);
            }
            series.Add(chartService.GetAccountExpensesPerMonth(id, DateTime.Now.Year));
            return new JsonResult(new ChartData(series));
        }

        #endregion
    }
}