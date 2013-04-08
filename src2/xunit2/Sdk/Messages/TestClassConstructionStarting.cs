﻿using Xunit.Abstractions;

namespace Xunit.Sdk
{
    /// <summary>
    /// Default implementation of <see cref="ITestClassConstructionStarting"/>.
    /// </summary>
    public class TestClassConstructionStarting : LongLivedMarshalByRefObject, ITestClassConstructionStarting
    {
        /// <inheritdoc/>
        public ITestCase TestCase { get; set; }

        /// <inheritdoc/>
        public string TestDisplayName { get; set; }
    }
}