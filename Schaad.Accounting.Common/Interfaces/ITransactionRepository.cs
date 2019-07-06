using System.Collections.Generic;
using Schaad.Accounting.Models;

namespace Schaad.Accounting.Interfaces
{
    public interface ITransactionRepository
    {
        List<Transaction> GetTransactionList();

        void SaveTransaction(Transaction transaction);

        Transaction GetTransaction(string id);
    }
}