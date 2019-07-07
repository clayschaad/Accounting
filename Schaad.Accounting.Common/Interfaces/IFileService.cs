using System.Collections.Generic;
using Schaad.Accounting.Datasets;
using Schaad.Finance.Api;
using Schaad.Finance.Api.Datasets;

namespace Schaad.Accounting.Interfaces
{
    public interface IFileService
    {
        string Backup();
        IReadOnlyList<MessageDataset> ImportAccountStatementFile(string filePath);
        IReadOnlyList<CreditCardTransaction> ImportCreditCardStatementFile(CreditCardProvider creditCardProvider, string filePath);
        byte[] GetTransactionListCsv(string accountId);
    }
}