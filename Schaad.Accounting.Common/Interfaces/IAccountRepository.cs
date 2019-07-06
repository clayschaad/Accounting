using System.Collections.Generic;
using Schaad.Accounting.Models;

namespace Schaad.Accounting.Interfaces
{
    public interface IAccountRepository
    {
        List<Account> GetAccountList();

        void SaveAccount(Account account);

        Account GetAccount(string id);

        Account GetAccountForBankAccountNumber(string bankAccountNumber);

        void SaveBankAccountBalance(string bankAccountNumber, decimal accountBalance);
    }
}