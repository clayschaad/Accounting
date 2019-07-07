using System.Collections.Generic;
using Schaad.Accounting.Datasets;
using Schaad.Accounting.Datasets.Reports;
using Schaad.Accounting.Models;
using Schaad.Finance.Api.Datasets;

namespace Schaad.Accounting.Interfaces
{
    public interface IViewService
    {
        AccountDataset GetAccountView(string id);

        List<AccountDataset> GetAccountViewList();

        BalanceDataset GetBalanceView();

        BalanceSheetDataset GetBalanceSheetView(int year);

        List<TransactionDataset> GetTransactionViewList();

        List<TransactionDataset> GetTransactionViewList(string accountId);

        List<BookingRuleDataset> GetBookingRuleViewList();

        List<BankTransaction> GetOpenBankTransactionList();

        List<Transaction> MatchOpenBankTransactions();

        List<Transaction> MatchCreditCardTransactions(string bankTransactionId, IReadOnlyList<CreditCardTransaction> creditCardTransactions);
    }
}