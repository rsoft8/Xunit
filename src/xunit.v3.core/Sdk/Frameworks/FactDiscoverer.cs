﻿using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit.Internal;
using Xunit.v3;

namespace Xunit.Sdk
{
	/// <summary>
	/// Implementation of <see cref="IXunitTestCaseDiscoverer"/> that supports finding test cases
	/// on methods decorated with <see cref="FactAttribute"/>.
	/// </summary>
	public class FactDiscoverer : IXunitTestCaseDiscoverer
	{
		/// <summary>
		/// Creates a single <see cref="XunitTestCase"/> for the given test method.
		/// </summary>
		/// <param name="discoveryOptions">The discovery options to be used.</param>
		/// <param name="testMethod">The test method.</param>
		/// <param name="factAttribute">The attribute that decorates the test method.</param>
		protected virtual IXunitTestCase CreateTestCase(
			_ITestFrameworkDiscoveryOptions discoveryOptions,
			_ITestMethod testMethod,
			_IAttributeInfo factAttribute)
		{
			Guard.ArgumentNotNull(discoveryOptions);
			Guard.ArgumentNotNull(testMethod);
			Guard.ArgumentNotNull(factAttribute);

			return new XunitTestCase(
				discoveryOptions.MethodDisplayOrDefault(),
				discoveryOptions.MethodDisplayOptionsOrDefault(),
				testMethod
			);
		}

		/// <summary>
		/// Discover test cases from a test method. By default, if the method is generic, or
		/// it contains arguments, returns a single <see cref="ExecutionErrorTestCase"/>;
		/// otherwise, it returns the result of calling <see cref="CreateTestCase"/>.
		/// </summary>
		/// <param name="discoveryOptions">The discovery options to be used.</param>
		/// <param name="testMethod">The test method the test cases belong to.</param>
		/// <param name="factAttribute">The fact attribute attached to the test method.</param>
		/// <returns>Returns zero or more test cases represented by the test method.</returns>
		public virtual ValueTask<IReadOnlyCollection<IXunitTestCase>> Discover(
			_ITestFrameworkDiscoveryOptions discoveryOptions,
			_ITestMethod testMethod,
			_IAttributeInfo factAttribute)
		{
			Guard.ArgumentNotNull(discoveryOptions);
			Guard.ArgumentNotNull(testMethod);
			Guard.ArgumentNotNull(factAttribute);

			IXunitTestCase testCase;

			if (testMethod.Method.GetParameters().Any())
				testCase = ErrorTestCase(discoveryOptions, testMethod, "[Fact] methods are not allowed to have parameters. Did you mean to use [Theory]?");
			else if (testMethod.Method.IsGenericMethodDefinition)
				testCase = ErrorTestCase(discoveryOptions, testMethod, "[Fact] methods are not allowed to be generic.");
			else
				testCase = CreateTestCase(discoveryOptions, testMethod, factAttribute);

			return new(new[] { testCase });
		}

		ExecutionErrorTestCase ErrorTestCase(
			_ITestFrameworkDiscoveryOptions discoveryOptions,
			_ITestMethod testMethod,
			string message) =>
				new(
					discoveryOptions.MethodDisplayOrDefault(),
					discoveryOptions.MethodDisplayOptionsOrDefault(),
					testMethod,
					message
				);
	}
}
