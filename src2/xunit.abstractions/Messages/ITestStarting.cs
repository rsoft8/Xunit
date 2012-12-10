﻿namespace Xunit.Abstractions
{
    public interface ITestStarting : ITestMessage
    {
        ITestCase TestCase { get; }
        string TestDisplayName { get; }

        // TODO: How do we differentiate a test?
    }
}
