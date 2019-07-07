using Schaad.Accounting.Datasets;

namespace Schaad.Accounting.Interfaces
{
    public interface ISettingsService
    {
        int GetYear();

        void SetYear(int year);

        bool TrySetYear(int year);

        string GetMandator();

        void SetMandator(string mandator);

        string GetYearMandatorPath();

        string GetLastYearMandatorPath();

        string GetBackupFilePath();

        string GetUploadPath();

        string GetCreditCardStatementPath();

        string GetDbPath();

        string GetLastYearDbPath();

        SettingsDataset GetSettings();
    }
}