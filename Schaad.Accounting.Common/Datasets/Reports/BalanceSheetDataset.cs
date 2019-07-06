using System.Collections.Generic;

namespace Schaad.Accounting.Datasets.Reports
{
    public class BalanceSheetDataset
    {
        public List<AccountDataset> ActivaAccountList { get; }

        public List<AccountDataset> IncomeAccountList { get; }

        public List<AccountDataset> ExpensesAccountList { get; }

        public decimal ProfitCHF { get; }

        public decimal LossCHF { get; }

        public int Year { get; }

        public BalanceSheetDataset(List<AccountDataset> activaAccountList, List<AccountDataset> incomeAccountList, List<AccountDataset> expensesAccountList, decimal profitCHF, decimal lossCHF, int year)
        {
            ActivaAccountList = activaAccountList;
            IncomeAccountList = incomeAccountList;
            ExpensesAccountList = expensesAccountList;
            ProfitCHF = profitCHF;
            LossCHF = lossCHF;
            Year = year;
        }
    }
}