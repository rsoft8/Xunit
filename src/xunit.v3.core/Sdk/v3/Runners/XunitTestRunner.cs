﻿using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Xunit.Sdk;

namespace Xunit.v3;

/// <summary>
/// The test runner for xUnit.net v3 tests.
/// </summary>
public class XunitTestRunner : TestRunner<XunitTestRunnerContext>
{
	/// <summary>
	/// Initializes a new instance of the <see cref="XunitTestRunner"/> class.
	/// </summary>
	protected XunitTestRunner()
	{ }

	/// <summary>
	/// Gets the singleton instance of the <see cref="XunitTestRunner"/>.
	/// </summary>
	public static XunitTestRunner Instance = new();

	/// <inheritdoc/>
	protected override async ValueTask<(decimal ExecutionTime, string Output)?> InvokeTestAsync(XunitTestRunnerContext ctxt)
	{
		var output = string.Empty;
		var testOutputHelper = TestContext.Current?.TestOutputHelper as TestOutputHelper;

		if (testOutputHelper != null)
			testOutputHelper.Initialize(ctxt.MessageBus, ctxt.Test);

		var executionTime = await InvokeTestMethodAsync(ctxt);

		if (testOutputHelper != null)
		{
			output = testOutputHelper.Output;
			testOutputHelper.Uninitialize();
		}

		return (executionTime, output);
	}

	/// <summary>
	/// Override this method to invoke the test method.
	/// </summary>
	/// <param name="ctxt">The context that describes the current test</param>
	/// <returns>Returns the execution time (in seconds) spent running the test method.</returns>
	protected virtual ValueTask<decimal> InvokeTestMethodAsync(XunitTestRunnerContext ctxt) =>
		XunitTestInvoker.Instance.RunAsync(
			ctxt.Test,
			ctxt.TestClass,
			ctxt.ConstructorArguments,
			ctxt.TestMethod,
			ctxt.TestMethodArguments,
			ctxt.BeforeAfterTestAttributes,
			ctxt.MessageBus,
			ctxt.Aggregator,
			ctxt.CancellationTokenSource
		);

	/// <summary>
	/// Runs the test.
	/// </summary>
	/// <param name="test">The test that this invocation belongs to.</param>
	/// <param name="messageBus">The message bus to report run status to.</param>
	/// <param name="testClass">The test class that the test method belongs to.</param>
	/// <param name="constructorArguments">The arguments to be passed to the test class constructor.</param>
	/// <param name="testMethod">The test method that will be invoked.</param>
	/// <param name="testMethodArguments">The arguments to be passed to the test method.</param>
	/// <param name="skipReason">The skip reason, if the test is to be skipped.</param>
	/// <param name="aggregator">The exception aggregator used to run code and collect exceptions.</param>
	/// <param name="cancellationTokenSource">The task cancellation token source, used to cancel the test run.</param>
	/// <param name="beforeAfterAttributes">The list of <see cref="BeforeAfterTestAttribute"/>s for this test.</param>
	/// <returns>Returns summary information about the test that was run.</returns>
	public ValueTask<RunSummary> RunAsync(
		_ITest test,
		IMessageBus messageBus,
		Type testClass,
		object?[] constructorArguments,
		MethodInfo testMethod,
		object?[]? testMethodArguments,
		string? skipReason,
		ExceptionAggregator aggregator,
		CancellationTokenSource cancellationTokenSource,
		IReadOnlyCollection<BeforeAfterTestAttribute> beforeAfterAttributes) =>
			RunAsync(new(test, messageBus, testClass, constructorArguments, testMethod, testMethodArguments, skipReason, aggregator, cancellationTokenSource, beforeAfterAttributes));

	/// <inheritdoc/>
	protected override void SetTestContext(
		XunitTestRunnerContext ctxt,
		TestEngineStatus testStatus,
		TestState? testState = null) =>
			TestContext.SetForTest(
				ctxt.Test,
				testStatus,
				ctxt.CancellationTokenSource.Token,
				testState,
				testStatus == TestEngineStatus.Initializing ? new TestOutputHelper() : TestContext.Current?.TestOutputHelper
			);
}
