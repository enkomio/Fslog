﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ES.Fslog.Loggers;

namespace ES.Fslog.UnitTests
{
    internal sealed class MockLogger : BaseLogger
    {
        public LogEvent LastLoggedEvent { get; private set; }

        public MockLogger()
        {
            this.LastLoggedEvent = null;
        }

        public override void WriteLogEvent(LogEvent logEvent)
        {
            if (this.EventLogLevelAllowed(logEvent.Level, LogLevel.Warning))
            {
                this.LastLoggedEvent = logEvent;    
            }
        }
    }
}