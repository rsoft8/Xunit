﻿using System.Linq;
using NSubstitute;
using Xunit;
using Xunit.Abstractions;
using Xunit.Runner.Common;
using Xunit.v3;

public class DelegatingFailSkipSinkTests
{
	IExecutionSink innerSink;
	DelegatingFailSkipSink sink;

	public DelegatingFailSkipSinkTests()
	{
		innerSink = Substitute.For<IExecutionSink>();
		sink = new DelegatingFailSkipSink(innerSink);
	}

	[Fact]
	public void OnITestSkipped_TransformsToITestFailed()
	{
		var inputMessage = Mocks.TestSkipped("The skipped test", "The skip reason");

		sink.OnMessage(inputMessage);

		var outputMessage = innerSink.Captured(x => x.OnMessage(null!)).Arg<ITestFailed>();
		Assert.Equal(inputMessage.Test, outputMessage.Test);
		Assert.Equal(0M, inputMessage.ExecutionTime);
		Assert.Empty(inputMessage.Output);
		Assert.Equal("FAIL_SKIP", outputMessage.ExceptionTypes.Single());
		Assert.Equal("The skip reason", outputMessage.Messages.Single());
		Assert.Empty(outputMessage.StackTraces.Single());
	}

	[Fact]
	public void OnITestCollectionFinished_CountsSkipsAsFails()
	{
		var inputMessage = Mocks.TestCollectionFinished(testsRun: 24, testsFailed: 8, testsSkipped: 3);

		sink.OnMessage(inputMessage);

		var outputMessage = innerSink.Captured(x => x.OnMessage(null!)).Arg<ITestCollectionFinished>();
		Assert.Equal(24, outputMessage.TestsRun);
		Assert.Equal(11, outputMessage.TestsFailed);
		Assert.Equal(0, outputMessage.TestsSkipped);
	}

	[Fact]
	public void OnITestAssemblyFinished_CountsSkipsAsFails()
	{
		var inputMessage = Mocks.TestAssemblyFinished(testsRun: 24, testsFailed: 8, testsSkipped: 3);

		sink.OnMessage(inputMessage);

		var outputMessage = innerSink.Captured(x => x.OnMessage(null!)).Arg<_TestAssemblyFinished>();
		Assert.Equal(24, outputMessage.TestsRun);
		Assert.Equal(11, outputMessage.TestsFailed);
		Assert.Equal(0, outputMessage.TestsSkipped);
	}
}
