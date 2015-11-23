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
        public void Log_a_critical_message_by_first_setting_the_log_source()
        {
            var sut = new LogProvider();
            
            var logSource = new StubLogSource();
            sut.AddLogSourceToLoggers(logSource);

            var mockLogger = new MockLogger();
            sut.AddLogger(mockLogger);

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

        [Fact]
        public void Ensure_that_only_one_instance_of_every_logger_is_added()
        {
            var sut = new LogProvider();
            var mockLogger = new MockLogger();
            sut.AddLogger(mockLogger);

            // add the same instance again
            sut.AddLogger(mockLogger);

            var logSource = new StubLogSource();
            sut.AddLogSourceToLoggers(logSource);

            logSource.SayPayAttentionTo("John");

            Assert.Equal(mockLogger.NumOfLoggedEvents, 1);
        }

        [Fact]
        public void Ensure_that_by_adding_logsource_and_logger_we_have_a_consistent_state()
        {
            var sut = new LogProvider();
            var mockLogger = new MockLogger();
            sut.AddLogger(mockLogger);

            var logSource = new StubLogSource();
            sut.AddLogSourceToLoggers(logSource);

            var consoleLogger = new ConsoleLogger(LogLevel.Informational);
            sut.AddLogger(consoleLogger);
            
            Assert.Equal(logSource.Loggers.Count, 2);
        }
    }
}
