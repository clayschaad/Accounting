using System;
using System.Collections.Generic;
using System.Linq;
using Schaad.Accounting.Interfaces;
using Schaad.Accounting.Models;

namespace Schaad.Accounting.Repositories
{
    public class TransactionRepository : BaseRepository, ITransactionRepository
    {
        private readonly IAccountRepository accountRepository;
        private readonly string TRANSACTIONS = "Transactions.xml";

        public TransactionRepository(ISettingsService settingsService, IAccountRepository accountRepository) : base(settingsService)
        {
            this.accountRepository = accountRepository;
        }

        /// <summary>
        /// Get transaction list
        /// </summary>
        public List<Transaction> GetTransactionList()
        {
            var transactionList = Load<List<Transaction>>(TRANSACTIONS);
            return transactionList ?? new List<Transaction>();
        }

        /// <summary>
        /// Save a transaction (insert/update)
        /// </summary>
        public void SaveTransaction(Transaction transaction)
        {
            // prevent saving fxrate for chf account
            var isFxAccount = accountRepository.GetAccount(transaction.OriginAccountId).IsFxAccount;

            // if source account is fx account, we pass a foreign value -> calc to chf
            if (isFxAccount && transaction.FxRate != 0)
            {
                transaction.Value = transaction.Value * transaction.FxRate.Value;
            }

            isFxAccount |= accountRepository.GetAccount(transaction.TargetAccountId).IsFxAccount;
            if (isFxAccount == false)
            {
                transaction.FxRate = null;
            }


            var transactionList = GetTransactionList();
            var existingTransaction = transactionList.FirstOrDefault(a => a.Id == transaction.Id);

            if (existingTransaction == null)
            {
                existingTransaction = new Transaction();
                transactionList.Add(existingTransaction);
                transaction.Id = Guid.NewGuid().ToString();
            }
            transaction.Copy(existingTransaction);
            Save(transactionList, TRANSACTIONS);
        }

        /// <summary>
        /// Get transaction
        /// </summary>
        public Transaction GetTransaction(string id)
        {
            var transactions = GetTransactionList();
            var transaction = transactions.FirstOrDefault(t => t.Id == id);

            // if source account is fx account, we save in chf currency -> calc to foreign currency for display
            var isFxAccount = accountRepository.GetAccount(transaction.OriginAccountId).IsFxAccount;
            if (isFxAccount && transaction.FxRate != 0)
            {
                transaction.Value = transaction.Value / transaction.FxRate.Value;
            }

            return transaction;
        }
    }
}