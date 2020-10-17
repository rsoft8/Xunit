﻿using System;
using System.Threading.Tasks;
using Xunit.Abstractions;
using Xunit.Internal;
using Xunit.v3;

namespace Xunit.Sdk
{
	/// <summary>
	/// A default implementation of <see cref="_ITestFramework"/> that tracks objects to be
	/// disposed when the framework is disposed. The discoverer and executor are automatically
	/// tracked for disposal, since those interfaces mandate an implementation of <see cref="IDisposable"/>.
	/// </summary>
	public abstract class TestFramework : _ITestFramework, IAsyncDisposable
	{
		bool disposed;
		_ISourceInformationProvider sourceInformationProvider = _NullSourceInformationProvider.Instance;

		/// <summary>
		/// Initializes a new instance of the <see cref="TestFramework"/> class.
		/// </summary>
		/// <param name="diagnosticMessageSink">The message sink which receives <see cref="IDiagnosticMessage"/> messages.</param>
		protected TestFramework(IMessageSink diagnosticMessageSink)
		{
			DiagnosticMessageSink = Guard.ArgumentNotNull(nameof(diagnosticMessageSink), diagnosticMessageSink);
		}

		/// <summary>
		/// Gets the message sink used to send diagnostic messages.
		/// </summary>
		public IMessageSink DiagnosticMessageSink { get; }

		/// <summary>
		/// Gets the disposal tracker for the test framework.
		/// </summary>
		protected DisposalTracker DisposalTracker { get; } = new DisposalTracker();

		/// <inheritdoc/>
		public _ISourceInformationProvider SourceInformationProvider
		{
			get => sourceInformationProvider;
			set => sourceInformationProvider = Guard.ArgumentNotNull(nameof(SourceInformationProvider), value);
		}

		/// <inheritdoc/>
		public virtual async ValueTask DisposeAsync()
		{
			if (disposed)
				throw new ObjectDisposedException(GetType().FullName);

			disposed = true;

			await ExtensibilityPointFactory.DisposeAsync();
			await DisposalTracker.DisposeAsync();
		}

		/// <summary>
		/// Override this method to provide the implementation of <see cref="ITestFrameworkDiscoverer"/>.
		/// </summary>
		/// <param name="assembly">The assembly that is being discovered.</param>
		/// <returns>Returns the test framework discoverer.</returns>
		protected abstract ITestFrameworkDiscoverer CreateDiscoverer(IAssemblyInfo assembly);

		/// <summary>
		/// Override this method to provide the implementation of <see cref="ITestFrameworkExecutor"/>.
		/// </summary>
		/// <param name="assembly">The assembly that is being executed.</param>
		/// <returns>Returns the test framework executor.</returns>
		protected abstract ITestFrameworkExecutor CreateExecutor(IReflectionAssemblyInfo assembly);

		/// <inheritdoc/>
		public ITestFrameworkDiscoverer GetDiscoverer(IAssemblyInfo assembly)
		{
			Guard.ArgumentNotNull(nameof(assembly), assembly);

			var discoverer = CreateDiscoverer(assembly);
			DisposalTracker.Add(discoverer);
			return discoverer;
		}

		/// <inheritdoc/>
		public ITestFrameworkExecutor GetExecutor(IReflectionAssemblyInfo assembly)
		{
			Guard.ArgumentNotNull(nameof(assembly), assembly);

			var executor = CreateExecutor(assembly);
			DisposalTracker.Add(executor);
			return executor;
		}
	}
}
