using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xunit.Internal;
using Xunit.Sdk;

namespace Xunit.v3;

/// <summary>
/// The test method runner for xUnit.net v3 tests.
/// </summary>
public class XunitTestMethodRunner : TestMethodRunner<XunitTestMethodRunnerContext, IXunitTestCase>
{
	/// <summary>
	/// Gets the singleton instance of the <see cref="XunitTestMethodRunner"/> class.
	/// </summary>
	public static XunitTestMethodRunner Instance { get; } = new();

	/// <summary>
	/// Runs the test test method.
	/// </summary>
	/// <param name="testClass">The test class that the test method belongs to. May be <c>null</c> for test cases
	/// that do not support classes.</param>
	/// <param name="testMethod">The test method to be run. May be <c>null</c> for test cases that do not support methods.</param>
	/// <param name="class">The CLR class that contains the test method. May be <c>null</c> for test cases that do not
	/// support classes and methods.</param>
	/// <param name="method">The test method that contains the tests to be run. May be <c>null</c> for test cases that do not
	/// support methods.</param>
	/// <param name="testCases">The test cases to be run. Cannot be empty.</param>
	/// <param name="messageBus">The message bus to report run status to.</param>
	/// <param name="aggregator">The exception aggregator used to run code and collect exceptions.</param>
	/// <param name="cancellationTokenSource">The task cancellation token source, used to cancel the test run.</param>
	/// <param name="constructorArguments">The constructor arguments for the test class.</param>
	public ValueTask<RunSummary> RunAsync(
		_ITestClass? testClass,
		_ITestMethod? testMethod,
		_IReflectionTypeInfo? @class,
		_IReflectionMethodInfo? method,
		IReadOnlyCollection<IXunitTestCase> testCases,
		IMessageBus messageBus,
		ExceptionAggregator aggregator,
		CancellationTokenSource cancellationTokenSource,
		object?[] constructorArguments)
	{
		Guard.ArgumentNotNull(testCases);
		Guard.ArgumentNotNull(messageBus);
		Guard.ArgumentNotNull(constructorArguments);

		return RunAsync(new(testClass, testMethod, @class, method, testCases, messageBus, aggregator, cancellationTokenSource, constructorArguments));
	}

	/// <inheritdoc/>
	protected override ValueTask<RunSummary> RunTestCaseAsync(
		XunitTestMethodRunnerContext ctxt,
		IXunitTestCase testCase) =>
			testCase.RunAsync(
				ctxt.MessageBus,
				ctxt.ConstructorArguments,
				ctxt.Aggregator,
				ctxt.CancellationTokenSource
			);
}
