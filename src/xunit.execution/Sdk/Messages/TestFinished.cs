﻿using System;
using Xunit.Abstractions;

#if XUNIT_CORE_DLL
namespace Xunit.Sdk
#else
namespace Xunit
#endif
{
    /// <summary>
    /// Default implementation of <see cref="ITestFinished"/>.
    /// </summary>
    public class TestFinished : TestMessage, ITestFinished
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TestFinished"/> class.
        /// </summary>
        public TestFinished(ITestCase testCase, string testDisplayName, decimal executionTime, string output)
            : base(testCase, testDisplayName)
        {
            ExecutionTime = executionTime;
            Output = output ?? String.Empty;
        }

        /// <inheritdoc/>
        public decimal ExecutionTime { get; private set; }

        /// <inheritdoc/>
        public string Output { get; private set; }
    }
}