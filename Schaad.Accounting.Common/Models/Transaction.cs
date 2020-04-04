using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Schaad.Accounting.Models
{
    public class Transaction
    {
        public string Id { get; set; }
        public string BankTransactionId { get; set; }

        /// <summary>
        /// Needed for booking rule
        /// </summary>
        public string BankTransactionText { get; set; }

        public string RelatedParty { get; set; }

        [Display(Name = "Valuta")]
        public DateTime ValueDate { get; set; }

        [Display(Name = "Buchung")]
        public DateTime BookingDate { get; set; }

        [Display(Name = "Buchungstext")]
        public string Text { get; set; }

        [Display(Name = "Betrag")]
        public decimal Value { get; set; }

        [Display(Name = "Habenkonto")]
        public string OriginAccountId { get; set; }

        [Display(Name = "Sollkonto")]
        public string TargetAccountId { get; set; }

        [Display(Name = "Währungskurs")]
        public decimal? FxRate { get; set; }

        public Transaction()
        {
        }

        public Transaction(BankTransaction bankTransaction, List<Account> accounts)
        {
            BankTransactionId = bankTransaction.Id;
            BankTransactionText = bankTransaction.Text;
            Value = bankTransaction.Value;
            ValueDate = bankTransaction.ValueDate;
            BookingDate = bankTransaction.BookingDate;

            // Depending on positiv or negative value, set the account
            var bankAccount = accounts.FirstOrDefault(a => a.BankAccountNumber == bankTransaction.BankAccountNumber);
            if (Value >= 0)
            {
                TargetAccountId = bankAccount.Id;
                RelatedParty = bankTransaction.Debtor;
            }
            else
            {
                OriginAccountId = bankAccount.Id;
                RelatedParty = bankTransaction.Creditor;
            }
        }

        /// <summary>
        /// Makes a copy
        /// </summary>
        public void Copy(Transaction target)
        {
            target.BankTransactionId = BankTransactionId;
            target.BankTransactionText = BankTransactionText;
            target.Id = Id;
            target.OriginAccountId = OriginAccountId;
            target.TargetAccountId = TargetAccountId;
            target.Text = Text;
            target.Value = Value;
            target.ValueDate = ValueDate;
            target.BookingDate = BookingDate > DateTime.MinValue ? BookingDate : ValueDate;
            target.FxRate = FxRate;
        }
    }
}