using Xunit.Abstractions;
using Xunit.Sdk;

namespace Parcorpus.UnitTests.Common.Orderers;

public class RandomOrderer : ITestCaseOrderer
{
    public IEnumerable<TTestCase> OrderTestCases<TTestCase>(
        IEnumerable<TTestCase> testCases) where TTestCase : ITestCase
    {
        return Environment.GetEnvironmentVariable("RandomizeTests") == "true"
            ? Randomize(testCases.ToList())
            : testCases;
    }
    
    private List<TTestCase> Randomize<TTestCase>(List<TTestCase> testCases) 
        where TTestCase : ITestCase
    {
        var result = new List<TTestCase>(testCases.Count);
        var randomizer = new Random();

        while (testCases.Count > 0)
        {
            var next = randomizer.Next(testCases.Count);
            result.Add(testCases[next]);
            testCases.RemoveAt(next);
        }

        return result;
    }
}