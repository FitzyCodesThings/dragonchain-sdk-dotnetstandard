﻿using System;
using Microsoft.Extensions.Logging;

namespace dragonchain_sdk.tests
{
    internal class NUnitLogger<T> : ILogger<T>, IDisposable
        {
            private readonly Action<string> output = Console.WriteLine;

            public void Dispose() { }

            public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception,
                Func<TState, Exception, string> formatter) => output(formatter(state, exception));

            public bool IsEnabled(LogLevel logLevel) => true;

            public IDisposable BeginScope<TState>(TState state) => this;
        }    
}