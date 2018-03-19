using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ES.Fslog.Loggers;

namespace ES.Fslog.UnitTests
{
    public class MockLogger : BaseLogger
    {
        public IEnumerable<LogEvent> Events { get; protected set; }
        public LogEvent LastLoggedEvent { get; protected set; }
        public Int32 NumOfLoggedEvents { get; protected set; }
        public LogLevel Level { get; set; }

        public MockLogger()
        {
            this.Events = new List<LogEvent>();
            this.LastLoggedEvent = null;
            this.NumOfLoggedEvents = 0;
            this.Level = LogLevel.Warning;
        }

        public Boolean ContainsLogMessage(String message)
        {
            return this.Events.Select(li => li.Message).Contains(message);
        }

        public override void WriteLogEvent(LogEvent logEvent)
        {
            if (this.EventLogLevelAllowed(logEvent.Level, this.Level))
            {
                this.LastLoggedEvent = logEvent;
                (this.Events as List<LogEvent>).Add(logEvent);
                this.NumOfLoggedEvents++;
            }
        }
    }

    public class SafeMockLogger : MockLogger
    {
        public SafeMockLogger() : base()
        {
            this.Events = new ConcurrentBag<LogEvent>();
        }

        public override void WriteLogEvent(LogEvent logEvent)
        {
            if (this.EventLogLevelAllowed(logEvent.Level, this.Level))
            {
                this.LastLoggedEvent = logEvent;
                (this.Events as ConcurrentBag<LogEvent>).Add(logEvent);
                this.NumOfLoggedEvents++;
            }
        }
    }
}
