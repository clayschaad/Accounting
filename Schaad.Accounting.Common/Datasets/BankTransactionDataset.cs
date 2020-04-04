using System;

namespace Schaad.Accounting.Datasets
{
    public class BankTransactionDataset
    {
        public string Id { get; set; }
        public DateTime BookingDate { get; set; }
        public DateTime ValueDate { get; set; }
        public decimal Value { get; set; }
        public string Text { get; set; }
        public string Debtor { get; set; }
        public string Creditor { get; set; }
    }
}