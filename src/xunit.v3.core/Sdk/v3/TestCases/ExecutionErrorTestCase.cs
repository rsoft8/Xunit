using System;
using System.Runtime.Serialization;
using System.Threading;
using System.Threading.Tasks;
using Xunit.Internal;
using Xunit.Sdk;

namespace Xunit.v3
{
	/// <summary>
	/// A simple implementation of <see cref="IXunitTestCase"/> that can be used to report an error
	/// rather than running a test.
	/// </summary>
	[Serializable]
	public class ExecutionErrorTestCase : XunitTestCase
	{
		string errorMessage;

		/// <inheritdoc/>
		protected ExecutionErrorTestCase(
			SerializationInfo info,
			StreamingContext context) :
				base(info, context)
		{
			errorMessage = Guard.NotNull("Could not retrieve ErrorMessage from serialization", info.GetValue<string>("ErrorMessage"));
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ExecutionErrorTestCase"/> class.
		/// </summary>
		/// <param name="diagnosticMessageSink">The message sink which receives <see cref="_DiagnosticMessage"/> messages.</param>
		/// <param name="defaultMethodDisplay">Default method display to use (when not customized).</param>
		/// <param name="defaultMethodDisplayOptions">Default method display options to use (when not customized).</param>
		/// <param name="testMethod">The test method.</param>
		/// <param name="errorMessage">The error message to report for the test.</param>
		public ExecutionErrorTestCase(
			_IMessageSink diagnosticMessageSink,
			TestMethodDisplay defaultMethodDisplay,
			TestMethodDisplayOptions defaultMethodDisplayOptions,
			_ITestMethod testMethod,
			string errorMessage)
				: base(diagnosticMessageSink, defaultMethodDisplay, defaultMethodDisplayOptions, testMethod)
		{
			this.errorMessage = Guard.ArgumentNotNull(nameof(errorMessage), errorMessage);
		}

		/// <summary>
		/// Gets the error message that will be display when the test is run.
		/// </summary>
		public string ErrorMessage
		{
			get => errorMessage;
			private set => errorMessage = Guard.ArgumentNotNull(nameof(ErrorMessage), value);
		}

		/// <inheritdoc/>
		public override void GetObjectData(
			SerializationInfo info,
			StreamingContext context)
		{
			base.GetObjectData(info, context);

			info.AddValue("ErrorMessage", ErrorMessage);
		}

		/// <inheritdoc/>
		public override Task<RunSummary> RunAsync(
			_IMessageSink diagnosticMessageSink,
			IMessageBus messageBus,
			object?[] constructorArguments,
			ExceptionAggregator aggregator,
			CancellationTokenSource cancellationTokenSource) =>
				new ExecutionErrorTestCaseRunner(this, messageBus, aggregator, cancellationTokenSource).RunAsync();
	}
}
