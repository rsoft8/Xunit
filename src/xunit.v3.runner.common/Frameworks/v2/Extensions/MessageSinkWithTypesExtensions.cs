﻿using Xunit.Abstractions;
using Xunit.Runner.v2;

/// <summary>
/// Extension methods for <see cref="IMessageSinkWithTypes"/>.
/// </summary>
public static class MessageSinkWithTypesExtensions
{
	/// <summary>
	/// Provides a simulation of <see cref="IMessageSink.OnMessage"/> for <see cref="IMessageSinkWithTypes"/>,
	/// to make it easier to directly dispatch messages from the runner.
	/// </summary>
	/// <param name="messageSink">The message sink</param>
	/// <param name="message">The message to be dispatched</param>
	/// <returns>The result of calling the message sink</returns>
	public static bool OnMessage(
		this IMessageSinkWithTypes messageSink,
		IMessageSinkMessage message)
	{
		Guard.ArgumentNotNull(nameof(messageSink), messageSink);
		Guard.ArgumentNotNull(nameof(message), message);

		return messageSink.OnMessageWithTypes(message, MessageSinkAdapter.GetImplementedInterfaces(message));
	}
}
