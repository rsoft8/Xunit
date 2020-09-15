using Xunit.Abstractions;
using Xunit.Runner.Common;

namespace Xunit.Runner.v2
{
	/// <summary>
	/// A message sent to implementations of <see cref="IRunnerReporter"/> when
	/// discovery is starting for a test assembly.
	/// </summary>
	public interface ITestAssemblyDiscoveryStarting : IMessageSinkMessage
	{
		/// <summary>
		/// Gets a flag which indicates whether the tests will be discovered and run in a
		/// separate app domain.
		/// </summary>
		AppDomainOption AppDomain { get; }

		/// <summary>
		/// Gets information about the assembly being discovered.
		/// </summary>
		XunitProjectAssembly Assembly { get; }

		/// <summary>
		/// Gets the options that will be used during discovery.
		/// </summary>
		ITestFrameworkDiscoveryOptions DiscoveryOptions { get; }

		/// <summary>
		/// Gets a flag which indicates whether shadow copies are being used. If app domains are
		/// not enabled, then this value is ignored.
		/// </summary>
		bool ShadowCopy { get; }
	}
}
