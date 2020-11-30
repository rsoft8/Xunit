﻿using Xunit.Internal;
using Xunit.v3;

namespace Xunit.Runner.Common
{
	/// <summary>
	/// Class that maps test runner messages to events.
	/// </summary>
	public class RunnerEventSink : _IMessageSink
	{
		/// <summary>
		/// Occurs when the runner is starting discovery for a given test assembly.
		/// </summary>
		public event MessageHandler<TestAssemblyDiscoveryFinished>? TestAssemblyDiscoveryFinishedEvent;

		/// <summary>
		/// Occurs when the runner has finished discovery for a given test assembly.
		/// </summary>
		public event MessageHandler<TestAssemblyDiscoveryStarting>? TestAssemblyDiscoveryStartingEvent;

		/// <summary>
		/// Occurs when the runner has finished executing the given test assembly.
		/// </summary>
		public event MessageHandler<TestAssemblyExecutionFinished>? TestAssemblyExecutionFinishedEvent;

		/// <summary>
		/// Occurs when the runner is starting to execution the given test assembly.
		/// </summary>
		public event MessageHandler<TestAssemblyExecutionStarting>? TestAssemblyExecutionStartingEvent;

		/// <summary>
		/// Occurs when the runner has finished executing all test assemblies.
		/// </summary>
		public event MessageHandler<TestExecutionSummaries>? TestExecutionSummariesEvent;

		/// <inheritdoc/>
		public bool OnMessage(_MessageSinkMessage message)
		{
			Guard.ArgumentNotNull(nameof(message), message);

			return
				message.DispatchWhen(TestAssemblyDiscoveryFinishedEvent) &&
				message.DispatchWhen(TestAssemblyDiscoveryStartingEvent) &&
				message.DispatchWhen(TestAssemblyExecutionFinishedEvent) &&
				message.DispatchWhen(TestAssemblyExecutionStartingEvent) &&
				message.DispatchWhen(TestExecutionSummariesEvent);
		}
	}
}
