using System;
using Schaad.Accounting.Models;

namespace Schaad.Accounting.Datasets
{
    public class TransactionDataset : Transaction
    {
        public Account OriginAccount { get; set; }

        public Account TargetAccount { get; set; }

        public TransactionDataset(Transaction transaction, Account originAccount, Account targetAccout)
        {
            this.BankTransactionId = transaction.BankTransactionId;
            this.Id = transaction.Id;
            this.OriginAccountId = transaction.OriginAccountId;
            this.TargetAccountId = transaction.TargetAccountId;
            this.Text = transaction.Text;
            this.Value = transaction.Value;
            this.FxRate = transaction.FxRate;
            this.ValueDate = transaction.ValueDate;
            this.BookingDate = transaction.BookingDate;
            this.OriginAccount = originAccount;
            this.TargetAccount = targetAccout;
        }

        public decimal GetValue(bool withFxRate)
        {
            if (withFxRate && FxRate != 0)
            {
                return Math.Round(Value / FxRate.Value, 2);
            }
            else
                return Value;
        }
    }
}