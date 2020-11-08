﻿using Xunit.Abstractions;
using Xunit.Internal;
using Xunit.v3;

namespace Xunit.Runner.Common
{
	/// <summary>
	/// Class that maps test framework execution messages to events.
	/// </summary>
	public class ExecutionEventSink : _IMessageSink
	{
		/// <summary>
		/// Occurs when a <see cref="IAfterTestFinished"/> message is received.
		/// </summary>
		public event MessageHandler<IAfterTestFinished>? AfterTestFinishedEvent;

		/// <summary>
		/// Occurs when a <see cref="IAfterTestStarting"/> message is received.
		/// </summary>
		public event MessageHandler<IAfterTestStarting>? AfterTestStartingEvent;

		/// <summary>
		/// Occurs when a <see cref="IBeforeTestFinished"/> message is received.
		/// </summary>
		public event MessageHandler<IBeforeTestFinished>? BeforeTestFinishedEvent;

		/// <summary>
		/// Occurs when a <see cref="IBeforeTestStarting"/> message is received.
		/// </summary>
		public event MessageHandler<IBeforeTestStarting>? BeforeTestStartingEvent;

		/// <summary>
		/// Occurs when a <see cref="ITestAssemblyCleanupFailure"/> message is received.
		/// </summary>
		public event MessageHandler<ITestAssemblyCleanupFailure>? TestAssemblyCleanupFailureEvent;

		/// <summary>
		/// Occurs when a <see cref="_TestAssemblyFinished"/> message is received.
		/// </summary>
		public event MessageHandler<_TestAssemblyFinished>? TestAssemblyFinishedEvent;

		/// <summary>
		/// Occurs when a <see cref="_TestAssemblyStarting"/> message is received.
		/// </summary>
		public event MessageHandler<_TestAssemblyStarting>? TestAssemblyStartingEvent;

		/// <summary>
		/// Occurs when a <see cref="ITestCaseCleanupFailure"/> message is received.
		/// </summary>
		public event MessageHandler<ITestCaseCleanupFailure>? TestCaseCleanupFailureEvent;

		/// <summary>
		/// Occurs when a <see cref="ITestCaseFinished"/> message is received.
		/// </summary>
		public event MessageHandler<ITestCaseFinished>? TestCaseFinishedEvent;

		/// <summary>
		/// Occurs when a <see cref="ITestCaseStarting"/> message is received.
		/// </summary>
		public event MessageHandler<ITestCaseStarting>? TestCaseStartingEvent;

		/// <summary>
		/// Occurs when a <see cref="ITestClassCleanupFailure"/> message is received.
		/// </summary>
		public event MessageHandler<ITestClassCleanupFailure>? TestClassCleanupFailureEvent;

		/// <summary>
		/// Occurs when a <see cref="ITestClassConstructionFinished"/> message is received.
		/// </summary>
		public event MessageHandler<ITestClassConstructionFinished>? TestClassConstructionFinishedEvent;

		/// <summary>
		/// Occurs when a <see cref="ITestClassConstructionStarting"/> message is received.
		/// </summary>
		public event MessageHandler<ITestClassConstructionStarting>? TestClassConstructionStartingEvent;

		/// <summary>
		/// Occurs when a <see cref="ITestClassDisposeFinished"/> message is received.
		/// </summary>
		public event MessageHandler<ITestClassDisposeFinished>? TestClassDisposeFinishedEvent;

		/// <summary>
		/// Occurs when a <see cref="ITestClassDisposeStarting"/> message is received.
		/// </summary>
		public event MessageHandler<ITestClassDisposeStarting>? TestClassDisposeStartingEvent;

		/// <summary>
		/// Occurs when a <see cref="ITestClassFinished"/> message is received.
		/// </summary>
		public event MessageHandler<ITestClassFinished>? TestClassFinishedEvent;

		/// <summary>
		/// Occurs when a <see cref="ITestClassStarting"/> message is received.
		/// </summary>
		public event MessageHandler<ITestClassStarting>? TestClassStartingEvent;

		/// <summary>
		/// Occurs when a <see cref="ITestCleanupFailure"/> message is received.
		/// </summary>
		public event MessageHandler<ITestCleanupFailure>? TestCleanupFailureEvent;

		/// <summary>
		/// Occurs when a <see cref="ITestCollectionCleanupFailure"/> message is received.
		/// </summary>
		public event MessageHandler<ITestCollectionCleanupFailure>? TestCollectionCleanupFailureEvent;

		/// <summary>
		/// Occurs when a <see cref="_TestCollectionFinished"/> message is received.
		/// </summary>
		public event MessageHandler<_TestCollectionFinished>? TestCollectionFinishedEvent;

		/// <summary>
		/// Occurs when a <see cref="_TestCollectionStarting"/> message is received.
		/// </summary>
		public event MessageHandler<_TestCollectionStarting>? TestCollectionStartingEvent;

		/// <summary>
		/// Occurs when a <see cref="ITestFailed"/> message is received.
		/// </summary>
		public event MessageHandler<ITestFailed>? TestFailedEvent;

		/// <summary>
		/// Occurs when a <see cref="ITestFinished"/> message is received.
		/// </summary>
		public event MessageHandler<ITestFinished>? TestFinishedEvent;

		/// <summary>
		/// Occurs when a <see cref="ITestMethodCleanupFailure"/> message is received.
		/// </summary>
		public event MessageHandler<ITestMethodCleanupFailure>? TestMethodCleanupFailureEvent;

		/// <summary>
		/// Occurs when a <see cref="ITestMethodFinished"/> message is received.
		/// </summary>
		public event MessageHandler<ITestMethodFinished>? TestMethodFinishedEvent;

		/// <summary>
		/// Occurs when a <see cref="ITestMethodStarting"/> message is received.
		/// </summary>
		public event MessageHandler<ITestMethodStarting>? TestMethodStartingEvent;

		/// <summary>
		/// Occurs when a <see cref="ITestOutput"/> message is received.
		/// </summary>
		public event MessageHandler<ITestOutput>? TestOutputEvent;

		/// <summary>
		/// Occurs when a <see cref="ITestPassed"/> message is received.
		/// </summary>
		public event MessageHandler<ITestPassed>? TestPassedEvent;

		/// <summary>
		/// Occurs when a <see cref="ITestSkipped"/> message is received.
		/// </summary>
		public event MessageHandler<ITestSkipped>? TestSkippedEvent;

		/// <summary>
		/// Occurs when a <see cref="ITestStarting"/> message is received.
		/// </summary>
		public event MessageHandler<ITestStarting>? TestStartingEvent;

		/// <inheritdoc/>
		public bool OnMessage(IMessageSinkMessage message)
		{
			Guard.ArgumentNotNull(nameof(message), message);

			return
				message.Dispatch(null, AfterTestFinishedEvent) &&
				message.Dispatch(null, AfterTestStartingEvent) &&
				message.Dispatch(null, BeforeTestFinishedEvent) &&
				message.Dispatch(null, BeforeTestStartingEvent) &&
				message.Dispatch(null, TestAssemblyCleanupFailureEvent) &&
				message.Dispatch(null, TestAssemblyFinishedEvent) &&
				message.Dispatch(null, TestAssemblyStartingEvent) &&
				message.Dispatch(null, TestCaseCleanupFailureEvent) &&
				message.Dispatch(null, TestCaseFinishedEvent) &&
				message.Dispatch(null, TestCaseStartingEvent) &&
				message.Dispatch(null, TestClassCleanupFailureEvent) &&
				message.Dispatch(null, TestClassConstructionFinishedEvent) &&
				message.Dispatch(null, TestClassConstructionStartingEvent) &&
				message.Dispatch(null, TestClassDisposeFinishedEvent) &&
				message.Dispatch(null, TestClassDisposeStartingEvent) &&
				message.Dispatch(null, TestClassFinishedEvent) &&
				message.Dispatch(null, TestClassStartingEvent) &&
				message.Dispatch(null, TestCleanupFailureEvent) &&
				message.Dispatch(null, TestCollectionCleanupFailureEvent) &&
				message.Dispatch(null, TestCollectionFinishedEvent) &&
				message.Dispatch(null, TestCollectionStartingEvent) &&
				message.Dispatch(null, TestFailedEvent) &&
				message.Dispatch(null, TestFinishedEvent) &&
				message.Dispatch(null, TestMethodCleanupFailureEvent) &&
				message.Dispatch(null, TestMethodFinishedEvent) &&
				message.Dispatch(null, TestMethodStartingEvent) &&
				message.Dispatch(null, TestOutputEvent) &&
				message.Dispatch(null, TestPassedEvent) &&
				message.Dispatch(null, TestSkippedEvent) &&
				message.Dispatch(null, TestStartingEvent);
		}
	}
}
