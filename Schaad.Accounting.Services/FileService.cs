using System.Collections.Generic;
using System.IO.Compression;
using System.Linq;
using System.Text;
using Schaad.Accounting.Common.Extensions;
using Schaad.Accounting.Datasets;
using Schaad.Accounting.Interfaces;
using Schaad.Finance.Api;
using Schaad.Finance.Api.Datasets;

namespace Schaad.Accounting.Services
{
    public class FileService : IFileService
    {
        private readonly IAccountRepository accountRepository;
        private readonly ITransactionRepository transactionsRepository;
        private readonly IBankTransactionRepository bankTransactionRepository;
        private readonly ISettingsService settingsService;
        private readonly IAccountStatementService accountStatementService;
        private readonly ICreditCardStatementService creditCardStatementService;

        public FileService(
            ISettingsService settingsService,
            IAccountRepository accountRepository,
            ITransactionRepository transactionsRepository,
            IBankTransactionRepository bankTransactionRepository,
            IAccountStatementService accountStatementService,
            ICreditCardStatementService creditCardStatementService)
        {
            this.settingsService = settingsService;
            this.accountRepository = accountRepository;
            this.transactionsRepository = transactionsRepository;
            this.bankTransactionRepository = bankTransactionRepository;
            this.accountStatementService = accountStatementService;
            this.creditCardStatementService = creditCardStatementService;
        }

        public string Backup()
        {
            var outpath = settingsService.GetBackupFilePath();
            var sourcepath = settingsService.GetYearMandatorPath();
            ZipFile.CreateFromDirectory(sourcepath, outpath);
            return $"Backup '{outpath}' erfolgreich erstellt";
        }

        // Upload account statement file (mt940, camt053)
        // http://www.mikesdotnetting.com/article/288/asp-net-5-uploading-files-with-asp-net-mvc-6
        public IReadOnlyList<MessageDataset> ImportAccountStatementFile(string filePath)
        {
            var messages = new List<MessageDataset>();
            var accountStatementResults = accountStatementService.ReadFile(filePath, Encoding.UTF8);

            var accountList = accountRepository.GetAccountList().Select(a => a.BankAccountNumber).ToList();
            foreach (var accountStatementResult in accountStatementResults)
            {
                var account = accountStatementResult.AccountStatement;

                if (accountStatementResult.IsSuccess == false)
                {
                    messages.Add(new MessageDataset($"Account {account.AccountNumber} NICHT importiert: {accountStatementResult.Error}", MessageStatus.Error));
                }

                if (accountStatementResult.AccountStatement.Transactions.Any() == false)
                {
                    continue;
                }

                var message = new MessageDataset($"Konto {account.AccountNumber}");
                messages.Add(message);

                // only accounts that are in select mandator
                if (accountList.Contains(account.AccountNumber))
                {
                    var transactionsThisYear = account.Transactions
                        .Where(t => t.BookingDate.Year == settingsService.GetYear())
                        .Select(
                            t => new BankTransactionDataset
                            {
                                Id = t.Id,
                                Value = (decimal)t.Value,
                                ValueDate = t.ValueDate,
                                BookingDate = t.BookingDate,
                                Text = t.Text,
                                Debtor = t.Debtor,
                                Creditor = t.Creditor
                            })
                        .ToList();
                    var count = bankTransactionRepository.SaveBankTransactionList(account.AccountNumber, transactionsThisYear);
                    accountRepository.SaveBankAccountBalance(account.AccountNumber, (decimal)account.EndBalance.Value);

                    var status = count == account.Transactions.Count ? MessageStatus.Success : MessageStatus.Info;
                    message.Add($"{count} von {account.Transactions.Count} Transaktion(en) importiert.", status);

                    // not imported transactions
                    var notImportedTransactions = account.Transactions.Where(t => transactionsThisYear.Select(tt => tt.Id).Contains(t.Id) == false);
                    foreach (var trx in notImportedTransactions)
                    {
                        message.Add($"Transaktion wegen BookingDatum {trx.BookingDate} nicht importiert: {trx.Text}", MessageStatus.Warning);
                    }
                }
                else
                {
                    message.Add($"Falscher Mandant: {account.Transactions.Count} Transaktion(en) nicht importiert.", MessageStatus.Warning);
                }
            }

            return messages;
        }

        public IReadOnlyList<CreditCardTransaction> ImportCreditCardStatementFile(CreditCardProvider creditCardProvider, string filePath)
        {
            var creditCardTransactions = creditCardStatementService.ReadFile(creditCardProvider, filePath, Encoding.UTF8);
            return creditCardTransactions;
        }

        public byte[] GetTransactionListCsv(string accountId)
        {
            var sb = new StringBuilder();
            var account = accountRepository.GetAccount(accountId);
            var transactions = transactionsRepository.GetTransactionList()
                .Where(t => t.OriginAccountId == accountId || t.TargetAccountId == accountId)
                .OrderBy(t => t.BookingDate)
                .ThenBy(t => t.ValueDate)
                .ThenBy(t => t.Value)
                .ToList();
            var balance = account.StartBalance;
            sb.AppendLine($"Buchungsdatum;Valuta;Buchungstext;Belastung;Gutschrift;Saldo {account.Currency}");
            sb.AppendLine($";;Startsaldo;;;{balance}");
            foreach (var trx in transactions)
            {
                var credit = "";
                var debit = "";

                if (trx.OriginAccountId == accountId)
                {
                    debit = trx.Value.ToFormattedString();
                    trx.Value *= -1;
                }
                else
                {
                    credit = trx.Value.ToFormattedString();
                }
                balance += trx.Value;
                sb.AppendLine($"{trx.BookingDate:dd.MM.yyyy};{trx.ValueDate:dd.MM.yyyy};{trx.Text};{debit};{credit};{balance.ToFormattedString()}");
            }
            var fileBytes = Encoding.GetEncoding("ISO-8859-1").GetBytes(sb.ToString());
            return fileBytes;
        }
    }
}