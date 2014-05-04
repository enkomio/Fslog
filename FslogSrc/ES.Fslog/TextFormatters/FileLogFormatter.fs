namespace ES.Fslog.TextFormatters

open System
open ES.Fslog

type FileLogFormatter() = 
    let dateFormat = "yyyy-MM-dd HH:mm:ss"
    
    member this.FormatMessage(logEvent: LogEvent) =
        String.Format("[{0}] {1} - {2} - {3}", logEvent.Level, logEvent.Timestamp.ToString(dateFormat), logEvent.SourceName, logEvent.Message)

    interface ITextFormatter with
        member this.FormatMessage(logEvent: LogEvent) =
            this.FormatMessage(logEvent)