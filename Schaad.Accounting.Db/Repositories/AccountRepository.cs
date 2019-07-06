using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Schaad.Accounting.Interfaces;
using Schaad.Accounting.Models;

namespace Schaad.Accounting.Repositories
{
    public class AccountRepository : BaseRepository, IAccountRepository
    {
        private readonly string ACCOUNTS = "Accounts.xml";

        public AccountRepository(ISettingsService settingsService) : base(settingsService)
        {
            EnsureFileExisits(ACCOUNTS);
        }


        /// <summary>
        /// Load accounts
        /// </summary>
        public List<Account> GetAccountList()
        {
            var accounts = Load<List<Account>>(ACCOUNTS);
            return accounts ?? new List<Account>();
        }

        /// <summary>
        /// Save an account (insert/update)
        /// </summary>
        public void SaveAccount(Account account)
        {
            var accounts = GetAccountList();
            var existingAccount = accounts.FirstOrDefault(a => a.Id == account.Id);

            if (existingAccount == null)
            {
                existingAccount = new Account();
                accounts.Add(existingAccount);
                account.Id = Guid.NewGuid().ToString();
            }
            account.Copy(existingAccount);
            Save(accounts, ACCOUNTS);
        }

        /// <summary>
        /// Get account
        /// </summary>
        public Account GetAccount(string id)
        {
            var accounts = GetAccountList();
            return accounts.FirstOrDefault(t => t.Id == id);
        }

        /// <summary>
        /// Get account
        /// </summary>
        public Account GetAccountForBankAccountNumber(string bankAccountNumber)
        {
            var accounts = GetAccountList();
            return accounts.SingleOrDefault(t => t.BankAccountNumber == bankAccountNumber);
        }

        public void SaveBankAccountBalance(string bankAccountNumber, decimal accountBalance)
        {
            var account = GetAccountForBankAccountNumber(bankAccountNumber);
            account.LastBankBalance = accountBalance;
            SaveAccount(account);
        }

        private new void EnsureFileExisits(string file)
        {
            string filePath = Path.Combine(settingsService.GetDbPath(), file);
            if (File.Exists(filePath) == false)
            {
                var lastYearFile = Path.Combine(settingsService.GetLastYearDbPath(), file);
                if (File.Exists(lastYearFile))
                    File.Copy(lastYearFile, filePath);

                // set start balance to last bank balance
                if (file.IndexOf("Accounts") > -1)
                {
                    var accounts = GetAccountList();
                    foreach (var account in accounts.Where(a => a.Class == ClassIds.Activa))
                    {
                        // bank accounts
                        if (account.LastBankBalance > 0)
                        {
                            account.StartBalance = account.LastBankBalance;
                            SaveAccount(account);
                        }
                        // cash accounts
                        else if (account.Number < 1010)
                        {
                            account.StartBalance = 0;
                            SaveAccount(account);
                        }
                    }
                }
            }
        }
    }
}