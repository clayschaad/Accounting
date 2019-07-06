using Schaad.Accounting.Models;

namespace Schaad.Accounting.Datasets
{
    public class AccountDataset : Account
    {
        public decimal Balance { get; }

        public decimal BalanceCHF { get; }

        public decimal StartBalanceCHF { get; }

        public string ClassName { get; }

        public string SubClassName { get; }

        public AccountDataset(Account account, decimal balance, decimal balanceCHF, decimal startBalanceCHF, string className, string subClassName)
        {
            LastBankBalance = account.LastBankBalance;
            StartBalance = account.StartBalance;
            StartBalanceCHF = startBalanceCHF;
            BankAccountNumber = account.BankAccountNumber;
            Currency = account.Currency;
            Id = account.Id;
            Name = account.Name;
            Number = account.Number;
            Balance = balance;
            BalanceCHF = balanceCHF;
            ClassName = className;
            SubClassName = subClassName;
        }

        public override string ToString()
        {
            return string.Format("Name: {0}, Number: {1}, Balance {2}", Name, Number, Balance);
        }
    }
}