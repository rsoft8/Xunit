﻿using System;
using System.Collections.Generic;
using System.Threading;
using Xunit.Abstractions;
using Xunit.Internal;
using Xunit.v3;

namespace Xunit.Runner.Common
{
	/// <summary>
	/// An implementation of <see cref="_IMessageSink"/> designed for test discovery for a
	/// single test assembly. The <see cref="Finished"/> event is triggered when discovery is complete.
	/// </summary>
	public class TestDiscoverySink : _IMessageSink, IDisposable
	{
		readonly Func<bool> cancelThunk;
		bool disposed;

		/// <summary>
		/// Initializes a new instance of the <see cref="TestDiscoverySink"/> class.
		/// </summary>
		/// <param name="cancelThunk">An optional thunk which can be used to control cancellation.</param>
		public TestDiscoverySink(Func<bool>? cancelThunk = null)
		{
			this.cancelThunk = cancelThunk ?? (() => false);

			DiscoverySink.TestCaseDiscoveryMessageEvent += args =>
			{
				Guard.ArgumentNotNull(nameof(args), args);

				TestCases.Add(args.Message.TestCase);
			};

			DiscoverySink.DiscoveryCompleteMessageEvent += args => Finished.Set();
		}

		/// <summary>
		/// Gets the event sink used to record discovery messages.
		/// </summary>
		protected DiscoveryEventSink DiscoverySink { get; } = new DiscoveryEventSink();

		/// <summary>
		/// Gets an event which is signaled once discovery is finished.
		/// </summary>
		public ManualResetEvent Finished { get; } = new ManualResetEvent(initialState: false);

		/// <summary>
		/// The list of discovered test cases.
		/// </summary>
		public List<ITestCase> TestCases { get; } = new List<ITestCase>();

		/// <inheritdoc/>
		public void Dispose()
		{
			if (disposed)
				throw new ObjectDisposedException(GetType().FullName);

			disposed = true;

			Finished.Dispose();
		}

		/// <inheritdoc/>
		public bool OnMessage(IMessageSinkMessage message)
		{
			Guard.ArgumentNotNull(nameof(message), message);

			return DiscoverySink.OnMessage(message) && !cancelThunk();
		}
	}
}
