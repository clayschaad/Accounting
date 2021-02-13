using System;
using System.Collections.Generic;
using System.Linq;
using Schaad.Accounting.Datasets;
using Schaad.Accounting.Datasets.Reports;
using Schaad.Accounting.Interfaces;
using Schaad.Accounting.Models;
using Schaad.Finance.Api;
using Schaad.Finance.Api.Datasets;

namespace Schaad.Accounting.Services
{
    public class ViewService : IViewService
    {
        private readonly IAccountRepository accountRepository;
        private readonly IBankTransactionRepository bankTransactionRepository;
        private readonly IBookingRuleRepository bookingRuleRepository;
        private readonly ISubclassRepository subclassRepository;
        private readonly ITransactionRepository transactionRepository;
        private readonly IFxService fxService;
        private readonly ISettingsService settingsService;

        public ViewService(
            IAccountRepository accountRepository,
            IBankTransactionRepository bankTransactionRepository,
            ITransactionRepository transactionRepository,
            ISubclassRepository subclassRepository,
            IBookingRuleRepository bookingRuleRepository,
            IFxService fxService,
            ISettingsService settingsService)
        {
            this.accountRepository = accountRepository;
            this.bankTransactionRepository = bankTransactionRepository;
            this.transactionRepository = transactionRepository;
            this.subclassRepository = subclassRepository;
            this.bookingRuleRepository = bookingRuleRepository;
            this.fxService = fxService;
            this.settingsService = settingsService;
        }

        /// <summary>
        /// Load accounts
        /// </summary>
        public AccountDataset GetAccountView(string id)
        {
            var accounts = GetAccountViewList();
            return accounts.Single(t => t.Id == id);
        }

        /// <summary>
        /// Load accounts and calculates their balances
        /// </summary>
        public List<AccountDataset> GetAccountViewList()
        {
            var accounts = accountRepository.GetAccountList();
            var transactionList = GetTransactionViewList();
            var subClassNameByNumber = subclassRepository.GetSubClassList().ToDictionary(s => s.Number, s => s.Name);
            var settings = settingsService.GetSettings();

            var accountList = accounts.Select(
                    a =>
                        new AccountDataset(
                            account: a,
                            balance: GetBalanceInAccountCurrency(a, transactionList),
                            balanceCHF: GetCurrentBalanceInCHF(a, transactionList),
                            startBalanceCHF: fxService.ConvertCurrency(a.StartBalance, a.Currency, "CHF", settings.FixerIoApiKey),
                            className: subclassRepository.GetClass(a.Class),
                            subClassName: subClassNameByNumber[a.SubClass]
                        )
                )
                .ToList();

            return accountList.OrderBy(a => a.Number).ToList();
        }

        public BalanceDataset GetBalanceView()
        {
            var accountList = GetAccountViewList();
            var balanceView = new BalanceDataset(
                    activaAccountList: accountList.Where(m => m.Class == ClassIds.Activa).ToList(),
                    passivaAccountList: accountList.Where(m => m.Class == ClassIds.Passiva).ToList(),
                    totalActivaCHF: Math.Abs(accountList.Where(m => m.Class == ClassIds.Activa).Sum(m => m.BalanceCHF)),
                    totalPassivaCHF: Math.Abs(accountList.Where(m => m.Class == ClassIds.Passiva).Sum(m => m.BalanceCHF))
                );

            return balanceView;
        }

        public BalanceSheetDataset GetBalanceSheetView(int year)
        {
            var accountList = GetAccountViewList();

            var settings = settingsService.GetSettings();
            var profit = Math.Abs(accountList.Where(m => m.Class == ClassIds.Income).Sum(m =>m.BalanceCHF));
            profit += Math.Abs(accountList.Where(m => m.Class == ClassIds.Activa).Sum(m => fxService.ConvertCurrency(m.StartBalance, m.Currency, "CHF", settings.FixerIoApiKey)));
            var loss = Math.Abs(accountList.Where(m => m.Class == ClassIds.Expenses).Sum(m => m.BalanceCHF));

            var balanceView = new BalanceSheetDataset(
                    activaAccountList: accountList.Where(m => m.Class == ClassIds.Activa).ToList(),
                    incomeAccountList: accountList.Where(m => m.Class == ClassIds.Income).ToList(),
                    expensesAccountList: accountList.Where(m => m.Class == ClassIds.Expenses).ToList(),
                    profitCHF: profit,
                    lossCHF: loss,
                    year: year
                );

            return balanceView;
        }

        private decimal GetBalanceInAccountCurrency(Account a, List<TransactionDataset> transactionList)
        {
            var balance = a.StartBalance
                            + transactionList.Where(t => t.TargetAccountId == a.Id).Sum(t => t.GetValue(a.IsFxAccount))
                            - transactionList.Where(t => t.OriginAccountId == a.Id).Sum(t => t.GetValue(a.IsFxAccount));
            return balance;
        }

        private decimal GetCurrentBalanceInCHF(Account a, List<TransactionDataset> transactionList)
        {
            var settings = settingsService.GetSettings();
            var balanceInAccountCurrency = GetBalanceInAccountCurrency(a, transactionList);
            var balanceInChf = fxService.ConvertCurrency(balanceInAccountCurrency, a.Currency, "CHF", settings.FixerIoApiKey);
            return balanceInChf;
        }

        /// <summary>
        /// Get transaction list with the origin and target account for each transaction
        /// </summary>
        public List<TransactionDataset> GetTransactionViewList()
        {
            var accountList = accountRepository.GetAccountList();
            var transactionList = transactionRepository.GetTransactionList();

            var transactionViewList = transactionList.Select(
                    t =>
                        new TransactionDataset(
                            t,
                            accountList.Single(a => a.Id == t.OriginAccountId),
                            accountList.Single(a => a.Id == t.TargetAccountId)
                        )
                )
                .ToList();

            return transactionViewList;
        }

        /// <summary>
        /// Get transaction list with the origin and target account for each transaction
        /// </summary>
        public List<TransactionDataset> GetTransactionViewList(string accountId)
        {
            var accountList = accountRepository.GetAccountList();
            var transactionList = transactionRepository.GetTransactionList().Where(t => t.OriginAccountId == accountId || t.TargetAccountId == accountId);

            var transactionViewList = transactionList.Select(
                    t =>
                        new TransactionDataset(
                            t,
                            accountList.Single(a => a.Id == t.OriginAccountId),
                            accountList.Single(a => a.Id == t.TargetAccountId)
                        )
                )
                .ToList();

            return transactionViewList;
        }


        /// <summary>
        /// Get booking rules with their account
        /// </summary>
        public List<BookingRuleDataset> GetBookingRuleViewList()
        {
            var accountList = accountRepository.GetAccountList();
            var bookinRuleList = bookingRuleRepository.GetBookingRuleList();

            return bookinRuleList.Select(
                    t =>
                        new BookingRuleDataset(
                            t,
                            accountList.Single(a => a.Id == t.AccountId).Name
                        )
                )
                .ToList();
        }

        /// <summary>
        /// Load open bank transactions
        /// </summary>
        public List<BankTransaction> GetOpenBankTransactionList()
        {
            var transactions = transactionRepository
                .GetTransactionList()
                .Where(t => string.IsNullOrEmpty(t.BankTransactionId) == false)
                .Select(t => t.BankTransactionId);

            var bankTransactions = bankTransactionRepository
                .GetBankTransactionList()
                .Where(b => b.Ignore == false && transactions.Contains(b.Id) == false)
                .ToList();

            return bankTransactions;
        }

        public List<Transaction> MatchOpenBankTransactions()
        {
            var newTransactionList = new List<Transaction>();
            var transactions = transactionRepository.GetTransactionList();
            var accounts = accountRepository.GetAccountList();
            var bookingRules = bookingRuleRepository.GetBookingRuleList();
            var bankTransactions = GetOpenBankTransactionList().OrderBy(t => t.ValueDate);
            foreach (var bankTransaction in bankTransactions)
            {
                var trx = new Transaction(bankTransaction, accounts);
                MatchBankTransactionByBookingRule(bankTransaction.Text, trx, bookingRules);
                MatchBankTransactionBySameAccountsLastMonth(bankTransaction, trx, transactions);
                MatchBankTransactionBySameValueLastMonth(bankTransaction, trx, transactions);
                newTransactionList.Add(trx);
            }
            return newTransactionList;
        }

        public List<Transaction> MatchCreditCardTransactions(string bankTransactionId, IReadOnlyList<CreditCardTransaction> creditCardTransactions)
        {
            var accounts = accountRepository.GetAccountList();
            var bankTrx = bankTransactionRepository.GetBankTransaction(bankTransactionId);
            var bookingRules = bookingRuleRepository.GetBookingRuleList();

            var trxList = new List<Transaction>();
            foreach (var creditCardTransaction in creditCardTransactions)
            {
                var trx = new Transaction(bankTrx, accounts);
                trx.Value = -1 * creditCardTransaction.Amount;
                trx.BookingDate = bankTrx.BookingDate;
                trx.ValueDate = creditCardTransaction.TransactionDate;

                MatchBankTransactionByBookingRule(creditCardTransaction.Transaction, trx, bookingRules);

                if (string.IsNullOrEmpty(trx.Text))
                {
                    trx.Text = creditCardTransaction.Transaction;
                }
                trxList.Add(trx);
            }

            return trxList;
        }

        private void MatchBankTransactionByBookingRule(string transactionText, Transaction trx, List<BookingRule> bookingRules)
        {
            if (string.IsNullOrEmpty(trx.Text) == false)
                return;

            // all booking rules that match the text
            var matchedRules = bookingRules.Where(r => transactionText.ToLowerInvariant().Contains(r.LookupText.ToLowerInvariant())).ToList();

            // then match by value
            var matchedRule = matchedRules.FirstOrDefault(r => r.LookupValue == Math.Abs(trx.Value));

            // if no found, take match without a value
            if (matchedRule == null)
            {
                matchedRule = matchedRules.FirstOrDefault(r => r.LookupValue == 0);
            }
            if (matchedRule != null)
            {
                if (string.IsNullOrEmpty(trx.OriginAccountId))
                {
                    trx.OriginAccountId = matchedRule.AccountId;
                }
                else
                {
                    trx.TargetAccountId = matchedRule.AccountId;
                }

                trx.Text = matchedRule.BookingText;
            }
        }

        private void MatchBankTransactionBySameAccountsLastMonth(BankTransaction bankTransaction, Transaction trx, List<Transaction> transactions)
        {
            if (string.IsNullOrEmpty(trx.Text) == false)
                return;

            var lastMonth = trx.ValueDate.AddMonths(-1);
            var lastMonthStart = new DateTime(lastMonth.Year, lastMonth.Month, 1);
            var lastMonthEnd = lastMonthStart.AddMonths(1).AddMinutes(-1);

            // Find all bookings for the same accounts of the last month
            var lastMonthTransactions = transactions.Where(
                    t =>
                        t.OriginAccountId == trx.OriginAccountId &&
                        t.TargetAccountId == trx.TargetAccountId &&
                        Math.Abs(t.Value) == Math.Abs(trx.Value) &&
                        t.ValueDate >= lastMonthStart &&
                        t.ValueDate <= lastMonthEnd)
                .ToList();

            MatchSameOrOtherDay(trx, lastMonthTransactions);
        }

        private void MatchBankTransactionBySameValueLastMonth(BankTransaction bankTransaction, Transaction trx, List<Transaction> transactions)
        {
            if (string.IsNullOrEmpty(trx.Text) == false)
                return;

            var lastMonth = trx.ValueDate.AddMonths(-1);
            var lastMonthStart = new DateTime(lastMonth.Year, lastMonth.Month, 1);
            var lastMonthEnd = lastMonthStart.AddMonths(1).AddMinutes(-1);

            // Find all bookings for the same amount of the last month
            var lastMonthTransactions = transactions.Where(
                    t =>
                        Math.Abs(t.Value) == Math.Abs(trx.Value) &&
                        t.ValueDate >= lastMonthStart &&
                        t.ValueDate <= lastMonthEnd)
                .ToList();

            MatchSameOrOtherDay(trx, lastMonthTransactions);
        }

        private void MatchSameOrOtherDay(Transaction trx, List<Transaction> lastMonthTransactions)
        {
            if (lastMonthTransactions.Any())
            {
                var lastMonthTransaction = lastMonthTransactions.Where(
                        t =>
                            t.ValueDate.Day == trx.ValueDate.Day)
                    .FirstOrDefault();

                if (lastMonthTransaction == null)
                {
                    lastMonthTransaction = lastMonthTransactions.Where(
                            t =>
                                t.ValueDate.Day >= trx.ValueDate.AddDays(-1).Day &&
                                t.ValueDate.Day <= trx.ValueDate.AddDays(1).Day)
                        .FirstOrDefault();
                }

                if (lastMonthTransaction != null)
                {
                    trx.Text = lastMonthTransaction.Text;
                    if (string.IsNullOrEmpty(trx.OriginAccountId))
                    {
                        trx.OriginAccountId = lastMonthTransaction.OriginAccountId;
                    }
                    else
                    {
                        trx.TargetAccountId = lastMonthTransaction.TargetAccountId;
                    }
                }
            }
        }
    }
}