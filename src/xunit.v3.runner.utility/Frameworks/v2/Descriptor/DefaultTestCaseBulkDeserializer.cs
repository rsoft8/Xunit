﻿using System.Collections.Generic;
using System.Linq;
using Xunit.Abstractions;

namespace Xunit.Internal
{
	/// <summary>
	/// INTERNAL CLASS. DO NOT USE.
	/// </summary>
	public class DefaultTestCaseBulkDeserializer : ITestCaseBulkDeserializer
	{
		readonly ITestFrameworkExecutor executor;

		/// <summary/>
		public DefaultTestCaseBulkDeserializer(ITestFrameworkExecutor executor)
		{
			Guard.ArgumentNotNull(nameof(executor), executor);

			this.executor = executor;
		}

		/// <inheritdoc/>
		public List<KeyValuePair<string?, ITestCase?>> BulkDeserialize(List<string> serializations) =>
			serializations
				.Select(serialization => executor.Deserialize(serialization))
				.Select(testCase => new KeyValuePair<string?, ITestCase?>(testCase?.UniqueID, testCase))
				.ToList();
	}
}
