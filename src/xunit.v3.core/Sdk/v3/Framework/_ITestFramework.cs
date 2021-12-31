using System;

namespace Xunit.v3
{
	/// <summary>
	/// Represents a test framework. There are two pieces to test frameworks: discovery and
	/// execution. The two factory methods represent these two pieces. Test frameworks may optionally
	/// implement either <see cref="IDisposable"/> or <see cref="IAsyncDisposable"/>. They may
	/// implement a constructor which is either empty, or takes a single <see cref="_IMessageSink"/>
	/// for diagnostic messages, or takes two instances of <see cref="_IMessageSink"/> for diagnostic
	/// messages and internal diagnostic messages, respectively.
	/// </summary>
	// TODO: Do we still think this is the right way to do constructors?
	public interface _ITestFramework
	{
		/// <summary>
		/// Get a test discoverer.
		/// </summary>
		/// <param name="assembly">The assembly to discover tests from.</param>
		/// <returns>The test discoverer.</returns>
		_ITestFrameworkDiscoverer GetDiscoverer(_IAssemblyInfo assembly);

		/// <summary>
		/// Get a test executor.
		/// </summary>
		/// <param name="assembly">The assembly to run tests from.</param>
		/// <returns>The test executor.</returns>
		_ITestFrameworkExecutor GetExecutor(_IReflectionAssemblyInfo assembly);
	}
}
