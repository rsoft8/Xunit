﻿using System;
using System.Diagnostics.CodeAnalysis;
using Xunit.Sdk;

namespace Xunit
{
    /// <summary>
    /// Attribute that is applied to a method to indicate that it is a fact that should be run
    /// by the test runner. It can also be extended to support a customized definition of a
    /// test method.
    /// </summary>
    [TestCaseDiscoverer("Xunit.Sdk.FactDiscoverer", "xunit2")]
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    [SuppressMessage("Microsoft.Performance", "CA1813:AvoidUnsealedAttributes", Justification = "This attribute is designed as an extensibility point.")]
    public class FactAttribute : Attribute
    {
        /// <summary>
        /// Gets the name of the test to be used when the test is skipped. Defaults to
        /// null, which will cause the fully qualified test name to be used.
        /// </summary>
        public virtual string DisplayName { get; set; }

        /// <summary>
        /// Marks the test so that it will not be run, and gets or sets the skip reason
        /// </summary>
        public virtual string Skip { get; set; }
    }
}