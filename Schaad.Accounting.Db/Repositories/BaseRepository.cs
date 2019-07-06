using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using Schaad.Accounting.Interfaces;

namespace Schaad.Accounting.Repositories
{
    public abstract class BaseRepository
    {
        protected readonly ISettingsService settingsService;

        protected BaseRepository(ISettingsService settingsService)
        {
            this.settingsService = settingsService;
        }

        protected void EnsureFileExisits(string fileName)
        {
            string filePath = Path.Combine(settingsService.GetDbPath(), fileName);
            if (File.Exists(filePath) == false)
            {
                var lastYearFile = Path.Combine(settingsService.GetLastYearDbPath(), fileName);
                if (File.Exists(lastYearFile))
                {
                    File.Copy(lastYearFile, filePath);
                }
            }
        }

        /// <summary>
        /// Save an object to an xml file
        /// </summary>
        protected void Save<T>(T obj, string fileName)
        {
            var filePath = Path.Combine(settingsService.GetDbPath(), fileName);
            using (var sww = new MemoryStream())
            {
                var settings = new XmlWriterSettings
                {
                    Encoding = Encoding.UTF8,
                    Indent = true
                };
                using (var writer = XmlWriter.Create(sww, settings))
                {
                    var xsSubmit = new XmlSerializer(typeof(T));
                    xsSubmit.Serialize(writer, obj);
                    var xml = Encoding.UTF8.GetString(sww.ToArray());
                    File.WriteAllText(filePath, xml);
                }
            }
        }

        /// <summary>
        /// Load an object from an xml file
        /// </summary>
        protected T Load<T>(string fileName)
        {
            var filePath = Path.Combine(settingsService.GetDbPath(), fileName);
            if (File.Exists(filePath) == false)
            {
                return default(T);
            }

            using (XmlReader reader = XmlReader.Create(filePath))
            {
                var serializer = new XmlSerializer(typeof(T));
                return (T)serializer.Deserialize(reader);
            }
        }
    }
}