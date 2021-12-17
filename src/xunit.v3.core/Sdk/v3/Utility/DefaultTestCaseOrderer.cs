﻿using System;
using System.Collections.Generic;
using System.Linq;
using Xunit.Internal;
using Xunit.Sdk;

namespace Xunit.v3
{
	/// <summary>
	/// Default implementation of <see cref="ITestCaseOrderer"/>. Orders tests in
	/// an unpredictable but stable order, so that repeated test runs of the
	/// identical test assembly run tests in the same order.
	/// </summary>
	public class DefaultTestCaseOrderer : ITestCaseOrderer
	{
		/// <inheritdoc/>
		public IReadOnlyCollection<TTestCase> OrderTestCases<TTestCase>(IReadOnlyCollection<TTestCase> testCases)
			where TTestCase : notnull, _ITestCase
		{
			var result = testCases.ToList();

			try
			{
				result.Sort(Compare);
			}
			catch (Exception ex)
			{
				TestContext.Current?.SendDiagnosticMessage("Exception thrown in DefaultTestCaseOrderer.OrderTestCases(); falling back to random order.{0}{1}", Environment.NewLine, ex);
				result = Randomize(result);
			}

			return result;
		}

		List<TTestCase> Randomize<TTestCase>(List<TTestCase> testCases)
		{
			var result = new List<TTestCase>(testCases.Count);
			var randomizer = new Random();

			while (testCases.Count > 0)
			{
				var next = randomizer.Next(testCases.Count);
				result.Add(testCases[next]);
				testCases.RemoveAt(next);
			}

			return result;
		}

		int Compare<TTestCase>(TTestCase x, TTestCase y)
			where TTestCase : notnull, _ITestCase
		{
			Guard.ArgumentNotNull(x.UniqueID);
			Guard.ArgumentNotNull(y.UniqueID);

			return string.CompareOrdinal(x.UniqueID, y.UniqueID);
		}
	}
}
