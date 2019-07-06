using System.Collections.Generic;
using Schaad.Accounting.Datasets.Charts;

namespace Schaad.Accounting.Interfaces
{
    public interface IChartService
    {
        List<DataSerie> GetSubClassExpensesPerMonth();

        List<DataSerie> GetAccountExpensesPerMonth();

        DataSerie GetAccountExpensesPerMonth(string accountId, int year);
    }
}