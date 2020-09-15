using System;
using System.Collections.Generic;
using Xunit.Abstractions;
using Xunit.Sdk;

#if XUNIT_FRAMEWORK
namespace Xunit.v2
#else
namespace Xunit.Runner.v2
#endif
{
	/// <summary>
	/// Default implementation of <see cref="IErrorMessage"/>.
	/// </summary>
#if XUNIT_FRAMEWORK
	public class ErrorMessage : IErrorMessage
#else
	public class ErrorMessage : LongLivedMarshalByRefObject, IErrorMessage
#endif
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="ErrorMessage"/> class.
		/// </summary>
		public ErrorMessage(
			IEnumerable<ITestCase> testCases,
			string?[] exceptionTypes,
			string[] messages,
			string?[] stackTraces,
			int[] exceptionParentIndices)
		{
			Guard.ArgumentNotNull(nameof(testCases), testCases);
			Guard.ArgumentNotNull(nameof(exceptionTypes), exceptionTypes);
			Guard.ArgumentNotNull(nameof(messages), messages);
			Guard.ArgumentNotNull(nameof(stackTraces), stackTraces);
			Guard.ArgumentNotNull(nameof(exceptionParentIndices), exceptionParentIndices);

			TestCases = testCases;
			ExceptionTypes = exceptionTypes;
			Messages = messages;
			StackTraces = stackTraces;
			ExceptionParentIndices = exceptionParentIndices;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ErrorMessage"/> class.
		/// </summary>
		public ErrorMessage(
			IEnumerable<ITestCase> testCases,
			Exception ex)
		{
			TestCases = testCases;

			var failureInfo = ExceptionUtility.ConvertExceptionToFailureInformation(ex);

			ExceptionTypes = failureInfo.ExceptionTypes;
			Messages = failureInfo.Messages;
			StackTraces = failureInfo.StackTraces;
			ExceptionParentIndices = failureInfo.ExceptionParentIndices;
		}

		/// <inheritdoc/>
		public string?[] ExceptionTypes { get; }

		/// <inheritdoc/>
		public string[] Messages { get; }

		/// <inheritdoc/>
		public string?[] StackTraces { get; }

		/// <inheritdoc/>
		public int[] ExceptionParentIndices { get; }

		/// <inheritdoc/>
		public IEnumerable<ITestCase> TestCases { get; }
	}
}
