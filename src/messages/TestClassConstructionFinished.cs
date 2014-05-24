﻿using Xunit.Abstractions;

#if XUNIT_CORE_DLL
namespace Xunit.Sdk
#else
namespace Xunit
#endif
{
    /// <summary>
    /// Default implementation of <see cref="ITestClassConstructionFinished"/>.
    /// </summary>
    public class TestClassConstructionFinished : TestMessage, ITestClassConstructionFinished
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TestClassConstructionFinished"/> class.
        /// </summary>
        public TestClassConstructionFinished(ITestCase testCase, string testDisplayName)
            : base(testCase, testDisplayName) { }
    }
}