﻿using System;
using System.Collections.Generic;
using System.Threading;
using Xunit.Abstractions;
using Xunit.Sdk;

public class SpyMessageSink<TFinalMessage> : LongLivedMarshalByRefObject, IMessageSink
{
    readonly Func<IMessageSinkMessage, bool> cancellationThunk;

    public SpyMessageSink(Func<IMessageSinkMessage, bool> cancellationThunk = null)
    {
        this.cancellationThunk = cancellationThunk ?? (msg => true);
    }

    public ManualResetEvent Finished = new ManualResetEvent(initialState: false);

    public List<IMessageSinkMessage> Messages = new List<IMessageSinkMessage>();

    public override void Dispose()
    {
        base.Dispose();

        Messages.ForEach(d => d.Dispose());
        Finished.Dispose();
    }

    public bool OnMessage(IMessageSinkMessage message)
    {
        Messages.Add(message);

        if (message is TFinalMessage)
            Finished.Set();

        return cancellationThunk(message);
    }
}
