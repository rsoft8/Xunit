using System;
using Xunit.Internal;
using Xunit.v3;

namespace Xunit.Runner.Common
{
	/// <summary>
	/// Reports that runner is about to start execution for a test assembly.
	/// </summary>
	public class TestAssemblyExecutionFinished : _MessageSinkMessage
	{
		XunitProjectAssembly? assembly;
		_ITestFrameworkExecutionOptions? executionOptions;
		ExecutionSummary? executionSummary;

		/// <summary>
		/// Gets information about the assembly being executed.
		/// </summary>
		public XunitProjectAssembly Assembly
		{
			get => assembly ?? throw new InvalidOperationException($"Attempted to get {nameof(Assembly)} on an uninitialized '{GetType().FullName}' object");
			set => assembly = Guard.ArgumentNotNull(nameof(Assembly), value);
		}

		/// <summary>
		/// Gets the options that was used during execution.
		/// </summary>
		public _ITestFrameworkExecutionOptions ExecutionOptions
		{
			get => executionOptions ?? throw new InvalidOperationException($"Attempted to get {nameof(ExecutionOptions)} on an uninitialized '{GetType().FullName}' object");
			set => executionOptions = Guard.ArgumentNotNull(nameof(ExecutionOptions), value);
		}

		/// <summary>
		/// Gets the summary of the execution results for the test assembly.
		/// </summary>
		public ExecutionSummary ExecutionSummary
		{
			get => executionSummary ?? throw new InvalidOperationException($"Attempted to get {nameof(ExecutionSummary)} on an uninitialized '{GetType().FullName}' object");
			set => executionSummary = Guard.ArgumentNotNull(nameof(ExecutionSummary), value);
		}
	}
}
