using System.Collections.Generic;
using System.Linq;
using Schaad.Accounting.Datasets;
using Schaad.Accounting.Interfaces;
using Schaad.Accounting.Models;

namespace Schaad.Accounting.Repositories
{
    public class BankTransactionRepository : BaseRepository, IBankTransactionRepository
    {
        private readonly string BANK_TRANSACTIONS = "BankTransactions.xml";

        public BankTransactionRepository(ISettingsService settingsService) : base(settingsService)
        {
        }

        /// <summary>
        /// Load bank transactions
        /// </summary>
        public List<BankTransaction> GetBankTransactionList()
        {
            var transactions = Load<List<BankTransaction>>(BANK_TRANSACTIONS);
            return transactions != null ? transactions : new List<BankTransaction>();
        }

        /// <summary>
        /// Save new bank transactions, ignore existing ones (no update)
        /// </summary>
        public int SaveBankTransactionList(string bankAccountNumber, List<BankTransactionDataset> bankTransactions)
        {
            var count = 0;
            var transactions = GetBankTransactionList();
            foreach (var bankTrx in bankTransactions)
            {
                var transaction = transactions.FirstOrDefault(t => t.Id == bankTrx.Id);
                if (transaction == null)
                {
                    transaction = new BankTransaction()
                    {
                        Id = bankTrx.Id,
                        BookingDate = bankTrx.BookingDate,
                        ValueDate = bankTrx.ValueDate,
                        Value = (decimal)bankTrx.Value,
                        Text = bankTrx.Text,
                        BankAccountNumber = bankAccountNumber,
                        Debtor = bankTrx.Debtor,
                        Creditor = bankTrx.Creditor,
                    };
                    transactions.Add(transaction);
                    count++;
                }
            }
            Save(transactions, BANK_TRANSACTIONS);
            return count;
        }

        /// <summary>
        /// Get bank transaction
        /// </summary>
        public BankTransaction GetBankTransaction(string id)
        {
            var transactions = GetBankTransactionList();
            return transactions.FirstOrDefault(t => t.Id == id);
        }
    }
}