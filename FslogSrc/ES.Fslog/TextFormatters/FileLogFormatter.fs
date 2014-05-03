namespace ES.Fslog.TextFormatters

open System
open ES.Fslog

type FileLogFormatter() = 
    
    member this.FormatMessage(logEvent: LogEvent) =
        String.Format("[{0}] {1} - {2} - {3}", logEvent.Level, logEvent.Timestamp.ToString("HH:mm:ss"), logEvent.SourceName, logEvent.Message)

    interface ITextFormatter with
        member this.FormatMessage(logEvent: LogEvent) =
            this.FormatMessage(logEvent)