﻿using Xunit.Abstractions;

#if XUNIT_CORE_DLL
namespace Xunit.Sdk
#else
namespace Xunit
#endif
{
    /// <summary>
    /// Default implementation of <see cref="ITestStarting"/>.
    /// </summary>
    public class TestStarting : TestMessage, ITestStarting
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TestStarting"/> class.
        /// </summary>
        public TestStarting(ITestCase testCase, string testDisplayName)
            : base(testCase, testDisplayName) { }
    }
}