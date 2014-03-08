﻿using System;
using System.Collections.Generic;
using System.Reflection;

namespace Xunit.Sdk
{
    /// <summary>
    /// Abstract attribute which represents a data source for a data theory.
    /// Data source providers derive from this attribute and implement GetData
    /// to return the data for the theory.
    /// </summary>
    [DataDiscoverer("Xunit.Sdk.DataDiscoverer", "xunit.execution")]
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
    public abstract class DataAttribute : Attribute
    {
        /// <summary>
        /// Returns the data to be used to test the theory.
        /// </summary>
        /// <param name="testMethod">The method that is being tested</param>
        /// <returns>One or more sets of theory data. Each invocation of the test method
        /// is represented by a single object array.</returns>
        public abstract IEnumerable<object[]> GetData(MethodInfo testMethod);
    }
}