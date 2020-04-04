using System;

namespace Schaad.Accounting.Models
{
    public class BankTransaction
    {
        /// <summary>
        /// Id
        /// </summary>
        public string Id { get; set; }

        public DateTime ValueDate { get; set; }

        public DateTime BookingDate { get; set; }

        public string Text { get; set; }

        public decimal Value { get; set; }

        public string BankAccountNumber { get; set; }

        public string Debtor { get; set; }
        public string Creditor { get; set; }

        public bool Ignore { get; set; }
    }
}