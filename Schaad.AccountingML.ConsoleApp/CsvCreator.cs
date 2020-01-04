using CsvHelper;
using Schaad.Accounting.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace Schaad.AccountingML.ConsoleApp
{
    public static class CsvCreator
    {
        public static void CreateCsv(string sourceFile, string targetFile)
        {
            var transactions = Load<List<Transaction>>(sourceFile);

            using (var writer = new StreamWriter(targetFile))
            {
                using (var csvWriter = new CsvWriter(writer))
                {
                    csvWriter.Configuration.Delimiter = ";";
                    csvWriter.Configuration.HasHeaderRecord = true;

                    csvWriter.WriteHeader<Data>();
                    csvWriter.NextRecord();
                    foreach (var transaction in transactions)
                    {
                        if (!string.IsNullOrEmpty(transaction.BankTransactionText))
                        {
                            csvWriter.WriteRecord(new Data
                            {
                                BankTransactionText = transaction.BankTransactionText.Replace("\n", " "),
                                Value = transaction.Value,
                                ValueDate = transaction.ValueDate,
                                Text = transaction.Text,
                                TargetAccountId = transaction.TargetAccountId
                            });
                            csvWriter.NextRecord();
                        }
                    }
                }
            }
        }

        private static T Load<T>(string sourceFile)
        {
            if (File.Exists(sourceFile) == false)
            {
                return default(T);
            }

            using (XmlReader reader = XmlReader.Create(sourceFile))
            {
                var serializer = new XmlSerializer(typeof(T));
                return (T)serializer.Deserialize(reader);
            }
        }
    }

    class Data
    {
        public string BankTransactionText { get; set; }
        public decimal Value { get; set; }
        public DateTime ValueDate { get; set; }
        public string Text { get; set; }
        public string TargetAccountId { get; set; }
    }
}
