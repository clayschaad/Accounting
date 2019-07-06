using System.Collections.Generic;
using Schaad.Accounting.Datasets;

namespace Schaad.Accounting.Interfaces
{
    public interface IFileService
    {
        string Backup();
        List<MessageDataset> ImportAccountStatementFile(string filePath);
        byte[] GetTransactionListCsv(string accountId);
    }
}