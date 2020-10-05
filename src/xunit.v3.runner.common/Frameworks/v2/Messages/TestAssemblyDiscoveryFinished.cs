using System.Collections.Generic;
using System.Linq;
using Xunit.Abstractions;
using Xunit.Internal;
using Xunit.Runner.Common;

namespace Xunit.Runner.v2
{
	/// <summary>
	/// Default implementation of <see cref="ITestAssemblyDiscoveryFinished"/>.
	/// </summary>
	public class TestAssemblyDiscoveryFinished : ITestAssemblyDiscoveryFinished, IMessageSinkMessageWithTypes
	{
		static readonly HashSet<string> interfaceTypes = new HashSet<string>(typeof(TestAssemblyDiscoveryFinished).GetInterfaces().Select(x => x.FullName!));

		/// <summary>
		/// Initializes a new instance of the <see cref="TestAssemblyDiscoveryFinished"/> class.
		/// </summary>
		/// <param name="assembly">Information about the assembly that is being discovered</param>
		/// <param name="discoveryOptions">The discovery options</param>
		/// <param name="testCasesDiscovered">The number of test cases discovered</param>
		/// <param name="testCasesToRun">The number of test cases to be run</param>
		public TestAssemblyDiscoveryFinished(
			XunitProjectAssembly assembly,
			ITestFrameworkDiscoveryOptions discoveryOptions,
			int testCasesDiscovered,
			int testCasesToRun)
		{
			Guard.ArgumentNotNull(nameof(assembly), assembly);
			Guard.ArgumentNotNull(nameof(discoveryOptions), discoveryOptions);

			Assembly = assembly;
			DiscoveryOptions = discoveryOptions;
			TestCasesDiscovered = testCasesDiscovered;
			TestCasesToRun = testCasesToRun;
		}

		/// <inheritdoc/>
		public XunitProjectAssembly Assembly { get; }

		/// <inheritdoc/>
		public ITestFrameworkDiscoveryOptions DiscoveryOptions { get; }

		/// <inheritdoc/>
		public HashSet<string> InterfaceTypes => interfaceTypes;

		/// <inheritdoc/>
		public int TestCasesDiscovered { get; }

		/// <inheritdoc/>
		public int TestCasesToRun { get; }
	}
}
