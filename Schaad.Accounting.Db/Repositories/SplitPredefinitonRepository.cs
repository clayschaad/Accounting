using System;
using System.Collections.Generic;
using System.Linq;
using Schaad.Accounting.Interfaces;
using Schaad.Accounting.Models;

namespace Schaad.Accounting.Repositories
{
    public class SplitPredefinitonRepository : BaseRepository, ISplitPredefinitonRepository
    {
        private readonly string SPLIT_PREDEFINITION = "SplitPredefinitions.xml";

        public SplitPredefinitonRepository(ISettingsService settingsService) : base(settingsService)
        {
            EnsureFileExisits(SPLIT_PREDEFINITION);
        }

        /// <summary>
        /// Load booking rules
        /// </summary>
        public List<SplitPredefinition> GetSplitPredefinitionList()
        {
            var definitions = Load<List<SplitPredefinition>>(SPLIT_PREDEFINITION);
            return definitions ?? new List<SplitPredefinition>();
        }

        /// <summary>
        /// Save a booking rule (insert/update)
        /// </summary>
        public void SaveSplitPredefinition(SplitPredefinition splitPredefinition)
        {
            var definitions = GetSplitPredefinitionList();
            var existingDefinition = definitions.FirstOrDefault(a => a.Id == splitPredefinition.Id);

            if (existingDefinition == null)
            {
                existingDefinition = new SplitPredefinition();
                definitions.Add(existingDefinition);
                splitPredefinition.Id = Guid.NewGuid().ToString();
            }
            splitPredefinition.Copy(existingDefinition);
            Save(definitions, SPLIT_PREDEFINITION);
        }
    }
}