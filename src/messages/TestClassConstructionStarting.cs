﻿using Xunit.Abstractions;

#if XUNIT_CORE_DLL
namespace Xunit.Sdk
#else
namespace Xunit
#endif
{
    /// <summary>
    /// Default implementation of <see cref="ITestClassConstructionStarting"/>.
    /// </summary>
    public class TestClassConstructionStarting : TestMessage, ITestClassConstructionStarting
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TestClassConstructionStarting"/> class.
        /// </summary>
        public TestClassConstructionStarting(ITestCase testCase, string testDisplayName)
            : base(testCase, testDisplayName) { }
    }
}