using System.Collections.Generic;

namespace Schaad.Accounting.Datasets.Charts
{
    public class DataSerie
    {
        public List<decimal> data { get; set; }
        public string name { get; set; }
        public string id { get; set; }


        public DataSerie(string name, List<decimal> data, string id)
        {
            this.name = name;
            this.data = data;
            this.id = id;
        }
    }
}