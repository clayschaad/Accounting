using System;
using System.IO;
using System.Linq;
using Schaad.Accounting.Datasets;
using Schaad.Accounting.Interfaces;

namespace Schaad.Accounting.Services
{
    public class SettingsService : ISettingsService
    {
        private readonly SettingsDataset settingsDataset;
        private string mandator = "Claudio Schaad";
        private int year = DateTime.Now.Year;

        public SettingsService(SettingsDataset settingsDataset)
        {
            this.settingsDataset = settingsDataset;
        }

        public int GetYear()
        {
            return year;
        }

        public void SetYear(int year)
        {
            this.year = year;
        }

        public bool TrySetYear(int year)
        {
            var oldYear = year;
            SetYear(year);
            if (Directory.GetFiles(GetDbPath()).Any() == false)
            {
                SetYear(oldYear);
                return false;
            }
            return true;
        }

        public string GetMandator()
        {
            return mandator;
        }

        public void SetMandator(string mandator)
        {
            this.mandator = mandator;
        }

        public string GetYearMandatorPath()
        {
            var path = Path.Combine(GetSettings().DataPath, year.ToString(), mandator);
            if (Directory.Exists(path) == false)
            {
                Directory.CreateDirectory(path);
            }
            return path;
        }

        public string GetLastYearMandatorPath()
        {
            var path = Path.Combine(GetSettings().DataPath, (year - 1).ToString(), mandator);
            return path;
        }

        public string GetBackupFilePath()
        {
            var backupPath = Path.Combine(GetSettings().DataPath, year.ToString(), "backup", mandator);
            if (Directory.Exists(backupPath) == false)
            {
                Directory.CreateDirectory(backupPath);
            }
            var backupFilePath = Path.Combine(backupPath, DateTime.Now.ToString("yyyyMMddHHmmss") + ".zip");
            return backupFilePath;
        }

        public string GetUploadPath()
        {
            var uploadPath = Path.Combine(GetYearMandatorPath(), "uploads");
            if (Directory.Exists(uploadPath) == false)
            {
                Directory.CreateDirectory(uploadPath);
            }
            return uploadPath;
        }

        public string GetCreditCardStatementPath()
        {
            var path = Path.Combine(GetUploadPath(), "creditCard");
            if (Directory.Exists(path) == false)
            {
                Directory.CreateDirectory(path);
            }
            return path;
        }

        public string GetDbPath()
        {
            var path = Path.Combine(GetYearMandatorPath(), "Data");
            if (Directory.Exists(path) == false)
            {
                Directory.CreateDirectory(path);
            }
            return path;
        }

        public string GetLastYearDbPath()
        {
            var path = Path.Combine(GetLastYearMandatorPath(), "Data");
            if (Directory.Exists(path) == false)
            {
                Directory.CreateDirectory(path);
            }
            return path;
        }

        public SettingsDataset GetSettings()
        {
            return settingsDataset;
        }
    }
}