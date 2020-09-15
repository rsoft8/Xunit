﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using Xunit.Abstractions;

namespace Xunit.Sdk
{
	/// <summary>
	/// A base class implementation of <see cref="ITestCase"/> which is based on test cases being
	/// related directly to test methods.
	/// </summary>
	public abstract class TestMethodTestCase : ITestCase, IDisposable
	{
		string? displayName;
		DisplayNameFormatter formatter;
		bool initialized;
		IMethodInfo? method;
		ITypeInfo[]? methodGenericTypes;
		string? skipReason;
		ISourceInformation? sourceInformation;
		ITestMethod? testMethod;
		Dictionary<string, List<string>>? traits;
		volatile string? uniqueID;

		/// <summary>
		/// Used for de-serialization.
		/// </summary>
		protected TestMethodTestCase()
		{
			formatter = new DisplayNameFormatter();
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="TestMethodTestCase"/> class.
		/// </summary>
		/// <param name="defaultMethodDisplay">Default method display to use (when not customized).</param>
		/// <param name="defaultMethodDisplayOptions">Default method display options to use (when not customized).</param>
		/// <param name="testMethod">The test method this test case belongs to.</param>
		/// <param name="testMethodArguments">The arguments for the test method.</param>
		/// <param name="skipReason">The reason for skipping the test.</param>
		protected TestMethodTestCase(
			TestMethodDisplay defaultMethodDisplay,
			TestMethodDisplayOptions defaultMethodDisplayOptions,
			ITestMethod testMethod,
			object?[]? testMethodArguments = null,
			string? skipReason = null)
		{
			DefaultMethodDisplay = defaultMethodDisplay;
			DefaultMethodDisplayOptions = defaultMethodDisplayOptions;
			this.testMethod = Guard.ArgumentNotNull(nameof(testMethod), testMethod);
			TestMethodArguments = testMethodArguments;
			this.skipReason = skipReason;

			formatter = new DisplayNameFormatter(defaultMethodDisplay, defaultMethodDisplayOptions);
		}

		/// <summary>
		/// Returns the base display name for a test ("TestClassName.MethodName").
		/// </summary>
		protected string BaseDisplayName
		{
			get
			{
				if (DefaultMethodDisplay == TestMethodDisplay.ClassAndMethod)
					return formatter.Format($"{TestMethod.TestClass.Class.Name}.{TestMethod.Method.Name}");

				return formatter.Format(TestMethod.Method.Name);
			}
		}

		/// <summary>
		/// Returns the default method display to use (when not customized).
		/// </summary>
		protected internal TestMethodDisplay DefaultMethodDisplay { get; private set; }

		/// <summary>
		/// Returns the default method display options to use (when not customized).
		/// </summary>
		protected internal TestMethodDisplayOptions DefaultMethodDisplayOptions { get; private set; }

		/// <inheritdoc/>
		public string DisplayName
		{
			get
			{
				EnsureInitialized();
				return displayName ?? throw new InvalidOperationException($"Attempted to get DisplayName on an uninitialized '{GetType().FullName}' object");
			}
			protected set
			{
				EnsureInitialized();
				displayName = Guard.ArgumentNotNull(nameof(DisplayName), value);
			}
		}

		/// <summary>
		/// Gets or sets the exception that happened during initialization. When this is set, then
		/// the test execution should fail with this exception.
		/// </summary>
		public Exception? InitializationException { get; protected set; }

		/// <inheritdoc/>
		public IMethodInfo Method
		{
			get
			{
				EnsureInitialized();
				return method ?? throw new InvalidOperationException($"Attempted to get Method on an uninitialized '{GetType().FullName}' object");
			}
			protected set
			{
				EnsureInitialized();
				method = Guard.ArgumentNotNull(nameof(Method), value);
			}
		}

		/// <summary>
		/// Gets the generic types that were used to close the generic test method, if
		/// applicable; <c>null</c>, if the test method was not an open generic.
		/// </summary>
		protected ITypeInfo[]? MethodGenericTypes
		{
			get
			{
				EnsureInitialized();
				return methodGenericTypes;
			}
		}

		/// <inheritdoc/>
		public string? SkipReason
		{
			get
			{
				EnsureInitialized();
				return skipReason;
			}
			protected set
			{
				EnsureInitialized();
				skipReason = value;
			}
		}

		/// <inheritdoc/>
		public ISourceInformation SourceInformation
		{
			get => sourceInformation ?? new SourceInformation();
			set => sourceInformation = Guard.ArgumentNotNull(nameof(SourceInformation), value);
		}

		/// <inheritdoc/>
		public ITestMethod TestMethod
		{
			get => testMethod ?? throw new InvalidOperationException($"Attempted to get TestMethod on an uninitialized '{GetType().FullName}' object");
			protected set => testMethod = Guard.ArgumentNotNull(nameof(TestMethod), value);
		}

		/// <inheritdoc/>
		public object?[]? TestMethodArguments { get; protected set; }

		/// <inheritdoc/>
		public Dictionary<string, List<string>> Traits
		{
			get
			{
				EnsureInitialized();
				return traits ?? throw new InvalidOperationException($"Attempted to get Traits on an uninitialized '{GetType().FullName}' object");
			}
			protected set
			{
				EnsureInitialized();
				traits = Guard.ArgumentNotNull(nameof(Traits), value);
			}
		}

		/// <inheritdoc/>
		public string UniqueID
		{
			get
			{
				EnsureInitialized();
				return
					uniqueID ??
					Interlocked.CompareExchange(ref uniqueID, GetUniqueID(), null) ??
					uniqueID;
			}
		}

		/// <inheritdoc/>
		public virtual void Dispose()
		{
			if (TestMethodArguments != null)
				foreach (var disposable in TestMethodArguments.OfType<IDisposable>())
					disposable.Dispose();
		}

		/// <summary>
		/// Call to ensure the object is fully initialized().
		/// </summary>
		protected void EnsureInitialized()
		{
			if (!initialized)
			{
				initialized = true;
				Initialize();
			}
		}

		/// <summary>
		/// Gets the unique ID for the test case.
		/// </summary>
		protected virtual string GetUniqueID()
		{
			using var idGenerator = new UniqueIDGenerator();
			var assemblyName = TestMethod.TestClass.TestCollection.TestAssembly.Assembly.Name;

			// Get just the assembly name (without version info) when obtained by reflection
			if (TestMethod.TestClass.TestCollection.TestAssembly.Assembly is IReflectionAssemblyInfo assembly)
				assemblyName = assembly.Assembly.GetName().Name ?? assemblyName;

			idGenerator.Add(assemblyName);
			idGenerator.Add(TestMethod.TestClass.Class.Name);
			idGenerator.Add(TestMethod.Method.Name);

			if (TestMethodArguments != null)
				idGenerator.Add(SerializationHelper.Serialize(TestMethodArguments));

			var genericTypes = MethodGenericTypes;
			if (genericTypes != null)
				for (var idx = 0; idx < genericTypes.Length; idx++)
					idGenerator.Add(TypeUtility.ConvertToSimpleTypeName(genericTypes[idx]));

			return idGenerator.Compute();
		}

		/// <summary>
		/// Called when initializing the test cases, either after constructor or de-serialization.
		/// Override this method to add additional initialization-time work.
		/// </summary>
		protected virtual void Initialize()
		{
			Traits = new Dictionary<string, List<string>>(StringComparer.OrdinalIgnoreCase);
			Method = TestMethod.Method;

			if (TestMethodArguments != null)
			{
				if (Method is IReflectionMethodInfo reflectionMethod)
				{
					try
					{
						TestMethodArguments = reflectionMethod.MethodInfo.ResolveMethodArguments(TestMethodArguments);
					}
					catch (Exception ex)
					{
						InitializationException = ex;
						TestMethodArguments = null;
						displayName = $"{BaseDisplayName}(???)";
					}
				}
			}

			if (TestMethodArguments != null && Method.IsGenericMethodDefinition)
			{
				methodGenericTypes = Method.ResolveGenericTypes(TestMethodArguments);
				Method = Method.MakeGenericMethod(MethodGenericTypes);
			}

			if (displayName == null)
				displayName = Method.GetDisplayNameWithArguments(BaseDisplayName, TestMethodArguments, MethodGenericTypes);
		}

		static void Write(Stream stream, string value)
		{
			var bytes = Encoding.UTF8.GetBytes(value);
			stream.Write(bytes, 0, bytes.Length);
			stream.WriteByte(0);
		}

		/// <inheritdoc/>
		public virtual void Serialize(IXunitSerializationInfo info)
		{
			Guard.ArgumentNotNull(nameof(info), info);

			info.AddValue("TestMethod", TestMethod);
			info.AddValue("TestMethodArguments", TestMethodArguments);
			info.AddValue("DefaultMethodDisplay", DefaultMethodDisplay.ToString());
			info.AddValue("DefaultMethodDisplayOptions", DefaultMethodDisplayOptions.ToString());
		}

		/// <inheritdoc/>
		public virtual void Deserialize(IXunitSerializationInfo info)
		{
			Guard.ArgumentNotNull(nameof(info), info);

			TestMethod = info.GetValue<ITestMethod>("TestMethod");
			TestMethodArguments = info.GetValue<object[]>("TestMethodArguments");
			DefaultMethodDisplay = (TestMethodDisplay)Enum.Parse(typeof(TestMethodDisplay), info.GetValue<string>("DefaultMethodDisplay"));
			DefaultMethodDisplayOptions = (TestMethodDisplayOptions)Enum.Parse(typeof(TestMethodDisplayOptions), info.GetValue<string>("DefaultMethodDisplayOptions"));
			formatter = new DisplayNameFormatter(DefaultMethodDisplay, DefaultMethodDisplayOptions);
		}
	}
}
