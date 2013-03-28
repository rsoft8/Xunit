using System;

namespace Xunit.Abstractions
{
    /// <summary>
    /// Represents a test framework. There are two pieces to test frameworks: discovery and
    /// execution. The two factory methods represent these two pieces.
    /// </summary>
    public interface ITestFramework : IDisposable
    {
        /// <summary>
        /// Get a test discoverer.
        /// </summary>
        /// <param name="assembly">The assembly from which to discover the tests.</param>
        /// <returns>The test discoverer.</returns>
        ITestFrameworkDiscoverer GetDiscoverer(IAssemblyInfo assembly);

        /// <summary>
        /// Get a test executor.
        /// </summary>
        /// <param name="assemblyFileName">The file path of the assembly to run tests from.</param>
        /// <returns>The test executor.</returns>
        ITestFrameworkExecutor GetExecutor(string assemblyFileName);
    }
}