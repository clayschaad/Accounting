namespace Schaad.Accounting.Datasets.Charts
{
    public class PieChartElement
    {
        public decimal y { get; set; }
        public string name { get; set; }
        public string id { get; set; }

        public PieChartElement(string name, decimal y)
        {
            this.name = name;
            this.y = y;
        }

        public PieChartElement(string name, decimal y, string id)
        {
            this.name = name;
            this.y = y;
            this.id = id;
        }
    }
}