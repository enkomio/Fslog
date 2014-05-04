using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Schema;
using ES.Fslog.Loggers;
using Xunit;

namespace ES.Fslog.UnitTests
{
    public class LogProviderTests
    {
        [Fact]
        public void Log_a_critical_message()
        {
            var sut = new LogProvider();
            var mockLogger = new MockLogger();
            sut.AddLogger(mockLogger);

            var logSource = new StubLogSource();
            sut.AddLogSourceToLoggers(logSource);

            logSource.SayPayAttentionTo("Dean");

            Assert.True(mockLogger.LastLoggedEvent.Message.Contains("Dean"));
            Assert.True(mockLogger.LastLoggedEvent.Level == LogLevel.Critical);
            Assert.True(mockLogger.LastLoggedEvent.SourceName.Equals(logSource.Name, StringComparison.Ordinal));
        }

        [Fact]
        public void Ensure_that_informational_message_are_filtered_for_warning_level_logger()
        {
            var sut = new LogProvider();
            var mockLogger = new MockLogger();
            sut.AddLogger(mockLogger);

            var logSource = new StubLogSource();
            sut.AddLogSourceToLoggers(logSource);

            logSource.SayHelloTo("John");

            Assert.Null(mockLogger.LastLoggedEvent);
        }
    }
}
