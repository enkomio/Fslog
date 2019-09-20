namespace ES.Fslog.Loggers

open System
open System.IO
open ES.Fslog
open ES.Fslog.TextFormatters

type TextWriterLogger(logLevel: LogLevel, textWriter: TextWriter, textFormatter: ITextFormatter) = 
    inherit BaseLogger()
        
    new(logLevel: LogLevel, textWriter) = new TextWriterLogger(logLevel, textWriter, new TextWriterLogFormatter())

    default val Level = logLevel with get

    override this.WriteLogEvent(logEvent: LogEvent) =
        if this.EventLogLevelAllowed(logEvent.Level, this.Level) then
            let formattedMessage = textFormatter.FormatMessage(logEvent)
            textWriter.WriteLine(formattedMessage)

    interface IDisposable with        
        member thid.Dispose() =
            textWriter.Dispose()