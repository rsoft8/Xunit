﻿using System;
using Xunit.v3;

namespace Xunit.Sdk
{
	/// <summary>
	/// Decorates an implementation of <see cref="_ITestFrameworkDiscoverer"/> that is used to
	/// determine which test framework is used to discover and run tests.
	/// </summary>
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
	public sealed class TestFrameworkDiscovererAttribute : Attribute
	{
		/// <summary>
		/// Initializes an instance of <see cref="TestFrameworkDiscovererAttribute"/>.
		/// </summary>
		/// <param name="typeName">The fully qualified type name of the discoverer
		/// (f.e., 'Xunit.Sdk.TestFrameworkDiscoverer')</param>
		/// <param name="assemblyName">The name of the assembly that the discoverer type
		/// is located in, without file extension (f.e., 'xunit.v3.core')</param>
		public TestFrameworkDiscovererAttribute(
			string typeName,
			string assemblyName)
		{ }

		/// <summary>
		/// Initializes an instance of <see cref="TestFrameworkDiscovererAttribute"/>.
		/// </summary>
		/// <param name="discovererType">The type of the discoverer</param>
		public TestFrameworkDiscovererAttribute(Type discovererType)
		{ }
	}
}
