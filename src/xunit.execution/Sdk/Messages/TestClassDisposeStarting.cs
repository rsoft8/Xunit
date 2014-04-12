﻿using Xunit.Abstractions;

#if XUNIT_CORE_DLL
namespace Xunit.Sdk
#else
namespace Xunit
#endif
{
    /// <summary>
    /// Default implementation of <see cref="ITestClassDisposeStarting"/>.
    /// </summary>
    public class TestClassDisposeStarting : TestMessage, ITestClassDisposeStarting
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TestClassDisposeStarting"/> class.
        /// </summary>
        public TestClassDisposeStarting(ITestCase testCase, string testDisplayName)
            : base(testCase, testDisplayName) { }
    }
}