using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ES.Fslog.Loggers;

namespace ES.Fslog.UnitTests
{
    public sealed class MockLogger : BaseLogger
    {
        public List<LogEvent> Events { get; private set; }
        public LogEvent LastLoggedEvent { get; private set; }
        public Int32 NumOfLoggedEvents { get; private set; }
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
                this.Events.Add(logEvent);
                this.NumOfLoggedEvents++;
            }
        }
    }
}
