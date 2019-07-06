using System.Collections.Generic;

namespace Schaad.Accounting.Datasets.Reports
{
    public class BalanceDataset
    {
        public List<AccountDataset> ActivaAccountList { get; set; }

        public List<AccountDataset> PassivaAccountList { get; set; }

        public decimal TotalActivaCHF { get; set; }

        public decimal TotalPassivaCHF { get; set; }

        public BalanceDataset(List<AccountDataset> activaAccountList, List<AccountDataset> passivaAccountList, decimal totalActivaCHF, decimal totalPassivaCHF)
        {
            ActivaAccountList = activaAccountList;
            PassivaAccountList = passivaAccountList;
            TotalActivaCHF = totalActivaCHF;
            TotalPassivaCHF = totalPassivaCHF;
        }
    }
}