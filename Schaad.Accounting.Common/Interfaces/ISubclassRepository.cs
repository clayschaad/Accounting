using System.Collections.Generic;
using Schaad.Accounting.Models;

namespace Schaad.Accounting.Interfaces
{
    public interface ISubclassRepository
    {
        List<SubClass> GetSubClassList();

        void SaveSubClass(SubClass subClass);

        SubClass GetSubClass(string id);

        Dictionary<int, string> GetClasses();

        string GetClass(int classId);
    }
}