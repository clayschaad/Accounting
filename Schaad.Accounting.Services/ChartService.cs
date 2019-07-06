using System;
using System.Collections.Generic;
using System.Linq;
using Schaad.Accounting.Datasets.Charts;
using Schaad.Accounting.Interfaces;

namespace Schaad.Accounting.Services
{
    public class ChartService : IChartService
    {
        private readonly IAccountRepository accountRepository;
        private readonly ISettingsService settingsService;
        private readonly ISubclassRepository subclassRepository;
        private readonly IViewService viewService;

        public ChartService(
            ISettingsService settingsService,
            IViewService viewService,
            IAccountRepository accountRepository,
            ISubclassRepository subclassRepository)
        {
            this.settingsService = settingsService;
            this.viewService = viewService;
            this.accountRepository = accountRepository;
            this.subclassRepository = subclassRepository;
        }

        public List<DataSerie> GetSubClassExpensesPerMonth()
        {
            var subclasses = subclassRepository.GetSubClassList();
            var transactions = viewService.GetTransactionViewList().Where(a => a.TargetAccount.Class == ClassIds.Expenses).ToList();
            var list = new List<DataSerie>();

            var newestTransaction = transactions.OrderByDescending(t => t.ValueDate).FirstOrDefault();
            var maxMonth = newestTransaction?.ValueDate.Month ?? 12;

            // Group by subclass
            foreach (var grp in transactions.GroupBy(a => a.TargetAccount.SubClass).Select(a => new {Key = a.Key, List = a.ToList()}))
            {
                // Sum subclass transactions per month
                var groupedByMonth = grp.List.GroupBy(g => g.ValueDate.Month).ToDictionary(g => g.Key, g => g.ToList().Sum(s => s.Value));
                EnsureEntryForEveryMonth(groupedByMonth, maxMonth);

                var subClass = subclasses.FirstOrDefault(s => s.Number == grp.List.First().TargetAccount.SubClass);
                list.Add(
                    new DataSerie(
                        subClass.Name,
                        groupedByMonth.OrderBy(o => o.Key).Select(o => o.Value).ToList(),
                        grp.List.First().TargetAccount.SubClass.ToString()
                    )
                );
            }
            return list;
        }

        public List<DataSerie> GetAccountExpensesPerMonth()
        {
            var list = new List<DataSerie>();
            var expensesAccounts = viewService.GetAccountViewList().Where(a => a.Class == ClassIds.Expenses);
            foreach (var account in expensesAccounts)
            {
                var serie = GetAccountExpensesPerMonth(account.Id, DateTime.Now.Year);
                if (serie != null)
                {
                    list.Add(serie);
                }
            }
            return list;
        }

        public DataSerie GetAccountExpensesPerMonth(string accountId, int year)
        {
            if (settingsService.TrySetYear(year))
            {
                var accountList = accountRepository.GetAccountList();
                var account = accountList.SingleOrDefault(a => a.Id == accountId);
                // perhaps we dont have the account for last year
                if (account == null)
                {
                    return null;
                }
                var transactions = viewService.GetTransactionViewList().Where(t => t.TargetAccountId == accountId).ToList();

                var newestTransaction = transactions.OrderByDescending(t => t.ValueDate).FirstOrDefault();
                var maxMonth = newestTransaction?.ValueDate.Month ?? 12;

                var groupedByMonth = transactions.GroupBy(g => g.ValueDate.Month).ToDictionary(g => g.Key, g => g.ToList().Sum(s => s.Value));
                EnsureEntryForEveryMonth(groupedByMonth, maxMonth);

                settingsService.SetYear(DateTime.Now.Year);

                return new DataSerie(
                    year.ToString(),
                    groupedByMonth.OrderBy(o => o.Key).Select(o => o.Value).ToList(),
                    account.Number.ToString()
                );
            }
            else
            {
                return new DataSerie(year.ToString(), new List<decimal>(), accountId);
            }
        }

        private void EnsureEntryForEveryMonth(Dictionary<int, decimal> values, int maxMonth = 12)
        {
            for (int i = 1; i <= maxMonth; i++)
            {
                if (values.ContainsKey(i) == false)
                {
                    values.Add(i, 0);
                }
            }
        }
    }
}