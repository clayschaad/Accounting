using System;
using System.Collections.Generic;
using System.Linq;
using Schaad.Accounting.Interfaces;
using Schaad.Accounting.Models;

namespace Schaad.Accounting.Repositories
{
    public class SubclassRepository : BaseRepository, ISubclassRepository
    {
        private readonly Dictionary<int, string> classes = new Dictionary<int, string>();
        private readonly string SUBCLASSES = "SubClasses.xml";

        public SubclassRepository(ISettingsService settingsService) : base(settingsService)
        {
            EnsureFileExisits(SUBCLASSES);

            classes.Add(ClassIds.Activa, "Aktiven");
            classes.Add(ClassIds.Passiva, "Passiven");
            classes.Add(ClassIds.Income, "Einnahmen");
            classes.Add(ClassIds.Expenses, "Ausgaben");
        }

        /// <summary>
        /// Load bokking texts
        /// </summary>
        public List<SubClass> GetSubClassList()
        {
            var subclasses = Load<List<SubClass>>(SUBCLASSES);
            return subclasses ?? new List<SubClass>();
        }

        /// <summary>
        /// Save a booking text (insert/update)
        /// </summary>
        public void SaveSubClass(SubClass subClass)
        {
            var subclasses = GetSubClassList();
            var existingSubClass = subclasses.FirstOrDefault(a => a.Id == subClass.Id);

            if (existingSubClass == null)
            {
                existingSubClass = new SubClass();
                subclasses.Add(existingSubClass);
                subClass.Id = Guid.NewGuid().ToString();
            }
            subClass.Copy(existingSubClass);
            Save(subclasses, SUBCLASSES);
        }

        /// <summary>
        /// Get subclass
        /// </summary>
        public SubClass GetSubClass(string id)
        {
            var subclasses = GetSubClassList();
            return subclasses.FirstOrDefault(t => t.Id == id);
        }

        public Dictionary<int, string> GetClasses()
        {
            return classes;
        }

        public string GetClass(int classId)
        {
            return classes[classId];
        }
    }
}