using System.Collections.Generic;
using Schaad.Accounting.Datasets;
using Schaad.Accounting.Models;

namespace Schaad.Accounting.Interfaces
{
    public interface IBankTransactionRepository
    {
        List<BankTransaction> GetBankTransactionList();

        int SaveBankTransactionList(string bankAccountNumber, List<BankTransactionDataset> bankTransactions);

        BankTransaction GetBankTransaction(string id);
    }
}