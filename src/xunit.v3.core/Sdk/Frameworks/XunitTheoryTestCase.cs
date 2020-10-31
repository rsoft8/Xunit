﻿using System;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using Xunit.Abstractions;
using Xunit.Runner.v2;
using Xunit.v3;

namespace Xunit.Sdk
{
	/// <summary>
	/// Represents a test case which runs multiple tests for theory data, either because the
	/// data was not enumerable or because the data was not serializable.
	/// </summary>
	public class XunitTheoryTestCase : XunitTestCase
	{
		/// <summary/>
		[EditorBrowsable(EditorBrowsableState.Never)]
		[Obsolete("Called by the de-serializer; should only be called by deriving classes for de-serialization purposes")]
		public XunitTheoryTestCase()
		{ }

		/// <summary>
		/// Initializes a new instance of the <see cref="XunitTheoryTestCase"/> class.
		/// </summary>
		/// <param name="diagnosticMessageSink">The message sink which receives <see cref="IDiagnosticMessage"/> messages.</param>
		/// <param name="defaultMethodDisplay">Default method display to use (when not customized).</param>
		/// <param name="defaultMethodDisplayOptions">Default method display options to use (when not customized).</param>
		/// <param name="testMethod">The method under test.</param>
		public XunitTheoryTestCase(
			_IMessageSink diagnosticMessageSink,
			TestMethodDisplay defaultMethodDisplay,
			TestMethodDisplayOptions defaultMethodDisplayOptions,
			ITestMethod testMethod)
				: base(diagnosticMessageSink, defaultMethodDisplay, defaultMethodDisplayOptions, testMethod)
		{ }

		/// <inheritdoc/>
		public override Task<RunSummary> RunAsync(
			_IMessageSink diagnosticMessageSink,
			IMessageBus messageBus,
			object?[] constructorArguments,
			ExceptionAggregator aggregator,
			CancellationTokenSource cancellationTokenSource) =>
				new XunitTheoryTestCaseRunner(
					this,
					DisplayName,
					SkipReason,
					constructorArguments,
					diagnosticMessageSink,
					messageBus,
					aggregator,
					cancellationTokenSource
				).RunAsync();
	}
}
