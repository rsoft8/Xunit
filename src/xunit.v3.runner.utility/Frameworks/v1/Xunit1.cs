﻿#if NETFRAMEWORK

using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using Xunit.Abstractions;
using Xunit.Runner.Common;
using Xunit.Runner.v2;
using Xunit.Sdk;

namespace Xunit
{
	/// <summary>
	/// This class be used to do discovery and execution of xUnit.net v1 tests
	/// using a reflection-based implementation of <see cref="IAssemblyInfo"/>.
	/// Runner authors are strongly encouraged to use <see cref="XunitFrontController"/>
	/// instead of using this class directly.
	/// </summary>
	public class Xunit1 : IFrontController
	{
		readonly AppDomainSupport appDomainSupport;
		readonly string assemblyFileName;
		readonly string? configFileName;
		readonly IMessageSink diagnosticMessageSink;
		bool disposed;
		IXunit1Executor? executor;
		readonly bool shadowCopy;
		readonly string? shadowCopyFolder;
		readonly ISourceInformationProvider sourceInformationProvider;
		readonly Stack<IDisposable> toDispose = new Stack<IDisposable>();

		/// <summary>
		/// Initializes a new instance of the <see cref="Xunit1"/> class.
		/// </summary>
		/// <param name="diagnosticMessageSink">The message sink which receives <see cref="IDiagnosticMessage"/> messages.</param>
		/// <param name="appDomainSupport">Determines whether tests should be run in a separate app domain.</param>
		/// <param name="sourceInformationProvider">Source code information provider.</param>
		/// <param name="assemblyFileName">The test assembly.</param>
		/// <param name="configFileName">The test assembly configuration file.</param>
		/// <param name="shadowCopy">If set to <c>true</c>, runs tests in a shadow copied app domain, which allows
		/// tests to be discovered and run without locking assembly files on disk.</param>
		/// <param name="shadowCopyFolder">The path on disk to use for shadow copying; if <c>null</c>, a folder
		/// will be automatically (randomly) generated</param>
		public Xunit1(
			IMessageSink diagnosticMessageSink,
			AppDomainSupport appDomainSupport,
			ISourceInformationProvider sourceInformationProvider,
			string assemblyFileName,
			string? configFileName = null,
			bool shadowCopy = true,
			string? shadowCopyFolder = null)
		{
			Guard.ArgumentNotNull(nameof(sourceInformationProvider), sourceInformationProvider);
			Guard.ArgumentNotNullOrEmpty(nameof(assemblyFileName), assemblyFileName);

			this.diagnosticMessageSink = diagnosticMessageSink;
			this.appDomainSupport = appDomainSupport;
			this.sourceInformationProvider = sourceInformationProvider;
			this.assemblyFileName = assemblyFileName;
			this.configFileName = configFileName;
			this.shadowCopy = shadowCopy;
			this.shadowCopyFolder = shadowCopyFolder;
		}

		/// <inheritdoc/>
		public bool CanUseAppDomains => true;

		IXunit1Executor Executor
		{
			get
			{
				if (executor == null)
					executor = CreateExecutor();

				return executor;
			}
		}

		/// <inheritdoc/>
		// This is not supported with v1, since there is no code in the remote AppDomain
		// that would give us this information.
		public string TargetFramework => string.Empty;

		/// <inheritdoc/>
		public string TestFrameworkDisplayName => Executor.TestFrameworkDisplayName;

		/// <summary>
		/// Creates a wrapper to call the Executor call from xUnit.net v1.
		/// </summary>
		/// <returns>The executor wrapper.</returns>
		protected virtual IXunit1Executor CreateExecutor() =>
			new Xunit1Executor(diagnosticMessageSink, appDomainSupport != AppDomainSupport.Denied, assemblyFileName, configFileName, shadowCopy, shadowCopyFolder);

		/// <inheritdoc/>
		public ITestCase? Deserialize(string value)
		{
			Guard.ArgumentNotNull(nameof(value), value);

			return SerializationHelper.Deserialize<ITestCase>(value);
		}

		/// <inheritdoc/>
		public void Dispose()
		{
			if (disposed)
				throw new ObjectDisposedException(GetType().FullName);

			disposed = true;

			foreach (var disposable in toDispose)
				disposable.Dispose();

			executor?.Dispose();
		}

		/// <summary>
		/// Starts the process of finding all xUnit.net v1 tests in an assembly.
		/// </summary>
		/// <param name="includeSourceInformation">Whether to include source file information, if possible.</param>
		/// <param name="messageSink">The message sink to report results back to.</param>
		public void Find(
			bool includeSourceInformation,
			IMessageSink messageSink)
		{
			Guard.ArgumentNotNull(nameof(messageSink), messageSink);

			Find(msg => true, includeSourceInformation, messageSink);
		}

		/// <inheritdoc/>
		void ITestFrameworkDiscoverer.Find(
			bool includeSourceInformation,
			IMessageSink? messageSink,
			ITestFrameworkDiscoveryOptions? discoveryOptions)
		{
			Guard.ArgumentNotNull(nameof(messageSink), messageSink);

			Find(msg => true, includeSourceInformation, messageSink);
		}

		/// <summary>
		/// Starts the process of finding all xUnit.net v1 tests in a class.
		/// </summary>
		/// <param name="typeName">The fully qualified type name to find tests in.</param>
		/// <param name="includeSourceInformation">Whether to include source file information, if possible.</param>
		/// <param name="messageSink">The message sink to report results back to.</param>
		public void Find(
			string typeName,
			bool includeSourceInformation,
			IMessageSink messageSink)
		{
			Find(msg => msg.TestCase.TestMethod.TestClass.Class.Name == typeName, includeSourceInformation, messageSink);
		}

		/// <inheritdoc/>
		void ITestFrameworkDiscoverer.Find(
			string typeName,
			bool includeSourceInformation,
			IMessageSink messageSink,
			ITestFrameworkDiscoveryOptions discoveryOptions)
		{
			Find(msg => msg.TestCase.TestMethod.TestClass.Class.Name == typeName, includeSourceInformation, messageSink);
		}

		void Find(
			Predicate<ITestCaseDiscoveryMessage> filter,
			bool includeSourceInformation,
			IMessageSink messageSink)
		{
			try
			{
				XmlNode? assemblyXml = null;

				var handler = new XmlNodeCallbackHandler(xml => { assemblyXml = xml; return true; });
				Executor.EnumerateTests(handler);

				if (assemblyXml != null)
				{
					foreach (var method in assemblyXml.SelectNodes("//method").Cast<XmlNode>())
					{
						var testCase = method.ToTestCase(assemblyFileName, configFileName);
						if (testCase != null)
						{
							if (includeSourceInformation)
								testCase.SourceInformation = sourceInformationProvider.GetSourceInformation(testCase);

							var message = new TestCaseDiscoveryMessage(testCase);
							if (filter(message))
								messageSink.OnMessage(message);
						}
					}
				}
			}
			finally
			{
				messageSink.OnMessage(new DiscoveryCompleteMessage());
			}
		}

		/// <summary>
		/// Starts the process of running all the xUnit.net v1 tests in the assembly.
		/// </summary>
		/// <param name="messageSink">The message sink to report results back to.</param>
		public void Run(IMessageSink messageSink)
		{
			var discoverySink = new TestDiscoverySink();
			toDispose.Push(discoverySink);

			Find(false, discoverySink);
			discoverySink.Finished.WaitOne();

			Run(discoverySink.TestCases, messageSink);
		}

		void ITestFrameworkExecutor.RunAll(
			IMessageSink messageSink,
			ITestFrameworkDiscoveryOptions discoveryOptions,
			ITestFrameworkExecutionOptions executionOptions)
		{
			Run(messageSink);
		}

		/// <summary>
		/// Starts the process of running all the xUnit.net v1 tests.
		/// </summary>
		/// <param name="testCases">The test cases to run; if null, all tests in the assembly are run.</param>
		/// <param name="messageSink">The message sink to report results back to.</param>
		public void Run(
			IEnumerable<ITestCase> testCases,
			IMessageSink messageSink)
		{
			var results = new Xunit1RunSummary();
			var environment = $"{IntPtr.Size * 8}-bit .NET {Environment.Version}";
			var firstTestCase = testCases.FirstOrDefault();
			var testCollection = firstTestCase == null ? null : firstTestCase.TestMethod.TestClass.TestCollection;

			if (testCollection != null)
			{
				try
				{
					if (messageSink.OnMessage(new TestAssemblyStarting(testCases, testCollection.TestAssembly, DateTime.Now, environment, TestFrameworkDisplayName)))
						results = RunTestCollection(testCollection, testCases, messageSink);
				}
				catch (Exception ex)
				{
					var failureInformation = Xunit1ExceptionUtility.ConvertToFailureInformation(ex);

					messageSink.OnMessage(new ErrorMessage(testCases, failureInformation.ExceptionTypes,
						failureInformation.Messages, failureInformation.StackTraces,
						failureInformation.ExceptionParentIndices));
				}
				finally
				{
					messageSink.OnMessage(new TestAssemblyFinished(testCases, testCollection.TestAssembly, results.Time, results.Total, results.Failed, results.Skipped));
				}
			}
		}

		void ITestFrameworkExecutor.RunTests(
			IEnumerable<ITestCase> testCases,
			IMessageSink messageSink,
			ITestFrameworkExecutionOptions executionOptions)
		{
			Run(testCases, messageSink);
		}

		Xunit1RunSummary RunTestCollection(
			ITestCollection testCollection,
			IEnumerable<ITestCase> testCases,
			IMessageSink messageSink)
		{
			var results = new Xunit1RunSummary
			{
				Continue = messageSink.OnMessage(new TestCollectionStarting(testCases, testCollection))
			};

			try
			{
				if (results.Continue)
					foreach (var testClassGroup in testCases.GroupBy(tc => tc.TestMethod.TestClass, Comparer.Instance))
					{
						var classResults = RunTestClass(testClassGroup.Key, testClassGroup.ToList(), messageSink);
						results.Aggregate(classResults);
						if (!classResults.Continue)
							break;
					}
			}
			finally
			{
				results.Continue = messageSink.OnMessage(new TestCollectionFinished(testCases, testCollection, results.Time, results.Total, results.Failed, results.Skipped)) && results.Continue;
			}

			return results;
		}

		Xunit1RunSummary RunTestClass(
			ITestClass testClass,
			IList<ITestCase> testCases,
			IMessageSink messageSink)
		{
			var handler = new TestClassCallbackHandler(testCases, messageSink);
			var results = handler.TestClassResults;
			results.Continue = messageSink.OnMessage(new TestClassStarting(testCases, testClass));

			try
			{
				if (results.Continue)
				{
					var methodNames = testCases.Select(tc => tc.TestMethod.Method.Name).ToList();
					Executor.RunTests(testClass.Class.Name, methodNames, handler);
					handler.LastNodeArrived.WaitOne();
				}
			}
			finally
			{
				results.Continue = messageSink.OnMessage(new TestClassFinished(testCases, testClass, results.Time, results.Total, results.Failed, results.Skipped)) && results.Continue;
			}

			return results;
		}

		/// <inheritdoc/>
		public string Serialize(ITestCase testCase) => SerializationHelper.Serialize(testCase);

		class Comparer : IEqualityComparer<ITestClass>
		{
			public static readonly Comparer Instance = new Comparer();

			public bool Equals(ITestClass? x, ITestClass? y) => x?.Class.Name == y?.Class.Name;

			public int GetHashCode(ITestClass obj) => obj.Class.Name.GetHashCode();
		}
	}
}

#endif
