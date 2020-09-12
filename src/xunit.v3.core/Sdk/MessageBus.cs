﻿using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading;
using Xunit.Abstractions;
using Xunit.v2;

namespace Xunit.Sdk
{
	/// <summary>
	/// This is an internal class, and is not intended to be called from end-user code.
	/// </summary>
	public class MessageBus : IMessageBus
	{
		volatile bool continueRunning = true;
		bool disposed;
		readonly IMessageSink messageSink;
		readonly ConcurrentQueue<IMessageSinkMessage> reporterQueue = new ConcurrentQueue<IMessageSinkMessage>();
		readonly Thread reporterThread;
		readonly AutoResetEvent reporterWorkEvent = new AutoResetEvent(false);
		volatile bool shutdownRequested;
		readonly bool stopOnFail;

		/// <summary/>
		public MessageBus(
			IMessageSink messageSink,
			bool stopOnFail = false)
		{
			this.messageSink = messageSink;
			this.stopOnFail = stopOnFail;

			reporterThread = new Thread(ReporterWorker);
			reporterThread.Start();
		}

		void DispatchMessages()
		{
			while (reporterQueue.TryDequeue(out var message))
				try
				{
					continueRunning &= messageSink.OnMessage(message);
				}
				catch (Exception ex)
				{
					try
					{
						var errorMessage = new ErrorMessage(Enumerable.Empty<ITestCase>(), ex);
						if (!messageSink.OnMessage(errorMessage))
							continueRunning = false;
					}
					catch { }
				}
		}

		/// <summary/>
		public void Dispose()
		{
			if (disposed)
				throw new ObjectDisposedException(GetType().FullName);

			disposed = true;

			shutdownRequested = true;

			reporterWorkEvent.Set();
			reporterThread.Join();
			reporterWorkEvent.Dispose();
		}

		/// <summary/>
		public bool QueueMessage(IMessageSinkMessage message)
		{
			Guard.ArgumentNotNull(nameof(message), message);

			if (shutdownRequested)
				throw new ObjectDisposedException("MessageBus");

			if (stopOnFail && message is ITestFailed)
				continueRunning = false;

			reporterQueue.Enqueue(message);
			reporterWorkEvent.Set();
			return continueRunning;
		}

		void ReporterWorker()
		{
			while (!shutdownRequested)
			{
				reporterWorkEvent.WaitOne();
				DispatchMessages();
			}

			// One final dispatch pass
			DispatchMessages();
		}
	}
}
