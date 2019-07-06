using System.Collections.Generic;
using Schaad.Accounting.Models;

namespace Schaad.Accounting.Interfaces
{
    public interface ISplitPredefinitonRepository
    {
        List<SplitPredefinition> GetSplitPredefinitionList();

        void SaveSplitPredefinition(SplitPredefinition splitPredefinition);
    }
}