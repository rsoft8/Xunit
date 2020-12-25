﻿using System.Collections.Generic;
using System.Linq;
using Xunit.Sdk;

namespace Xunit.v3
{
	public class CulturedTheoryAttributeDiscoverer : TheoryDiscoverer
	{
		public CulturedTheoryAttributeDiscoverer(_IMessageSink diagnosticMessageSink)
			: base(diagnosticMessageSink) { }

		protected override IEnumerable<IXunitTestCase> CreateTestCasesForDataRow(
			_ITestFrameworkDiscoveryOptions discoveryOptions,
			_ITestMethod testMethod,
			_IAttributeInfo theoryAttribute,
			object?[] dataRow)
		{
			var cultures = GetCultures(theoryAttribute);

			return cultures.Select(
				culture => new CulturedXunitTestCase(
					DiagnosticMessageSink,
					discoveryOptions.MethodDisplayOrDefault(),
					discoveryOptions.MethodDisplayOptionsOrDefault(),
					testMethod,
					culture,
					dataRow
				)
			).ToList();
		}

		protected override IEnumerable<IXunitTestCase> CreateTestCasesForTheory(
			_ITestFrameworkDiscoveryOptions discoveryOptions,
			_ITestMethod testMethod,
			_IAttributeInfo theoryAttribute)
		{
			var cultures = GetCultures(theoryAttribute);
			return cultures.Select(
				culture => new CulturedXunitTheoryTestCase(
					DiagnosticMessageSink,
					discoveryOptions.MethodDisplayOrDefault(),
					discoveryOptions.MethodDisplayOptionsOrDefault(),
					testMethod,
					culture
				)
			).ToList();
		}

		static string[] GetCultures(_IAttributeInfo culturedTheoryAttribute)
		{
			var ctorArgs = culturedTheoryAttribute.GetConstructorArguments().ToArray();
			var cultures = Reflector.ConvertArguments(ctorArgs, new[] { typeof(string[]) }).Cast<string[]>().Single();

			if (cultures == null || cultures.Length == 0)
				cultures = new[] { "en-US", "fr-FR" };

			return cultures;
		}
	}
}
