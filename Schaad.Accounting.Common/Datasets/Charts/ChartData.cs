using System.Collections.Generic;
using System.Linq;

namespace Schaad.Accounting.Datasets.Charts
{
    public class ChartData
    {
        public List<DataSerie> series { get; set; }

        public decimal median { get; set; }

        public ChartData(List<DataSerie> series)
        {
            this.series = series;

            var count = series.Sum(s => s.data.Count);
            var sum = series.Sum(s => s.data.Sum());
            this.median = sum / count;
        }
    }
}