using System;
using System.Collections.Generic;
using Check_n_Cheer.Models;

namespace Check_n_Cheer.Interfaces
{
    public interface ITestResultRepository
    {
        void AddTestResult(TestResult result);
        TestResult GetTestResult(Guid id);
        List<TestResult> GetTestResults();
        List<TestResult> GetTestResultsForStudent(Guid studentId);
        List<TestResult> GetTestResultsForTest(Guid testId);
        void RemoveTestResult(Guid id);
    }
}
