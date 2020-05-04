using Check_n_Cheer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Check_n_Cheer.Interfaces
{
    public interface ITestRepository
    {
        Test GetTest(Guid id);
        Test GetByName(string name);
        List<Test> GetTests();
        List<Test> GetTests(Guid teacherId);
        void UpdateTest(Guid id, Test updatedTest);
        void RemoveTest(Guid id);
        void AddTest(Test test);
    }
}
