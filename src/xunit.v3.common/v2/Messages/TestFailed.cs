﻿using System;
using Xunit.Abstractions;
using Xunit.Internal;

namespace Xunit.Runner.v2
{
	/// <summary>
	/// Default implementation of <see cref="ITestFailed"/>.
	/// </summary>
	public class TestFailed : TestResultMessage, ITestFailed
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="TestFailed"/> class.
		/// </summary>
		public TestFailed(
			ITest test,
			decimal executionTime,
			string? output,
			string?[] exceptionTypes,
			string[] messages,
			string?[] stackTraces,
			int[] exceptionParentIndices)
				: base(test, executionTime, output)
		{
			Guard.ArgumentNotNull(nameof(exceptionTypes), exceptionTypes);
			Guard.ArgumentNotNull(nameof(messages), messages);
			Guard.ArgumentNotNull(nameof(stackTraces), stackTraces);
			Guard.ArgumentNotNull(nameof(exceptionParentIndices), exceptionParentIndices);

			StackTraces = stackTraces;
			Messages = messages;
			ExceptionTypes = exceptionTypes;
			ExceptionParentIndices = exceptionParentIndices;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="TestFailed"/> class.
		/// </summary>
		public TestFailed(
			ITest test,
			decimal executionTime,
			string? output,
			Exception ex)
				: base(test, executionTime, output)
		{
			Guard.ArgumentNotNull(nameof(ex), ex);

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
	}
}
