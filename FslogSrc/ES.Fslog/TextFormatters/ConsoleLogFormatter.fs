namespace ES.Fslog.TextFormatters

open System
open ES.Fslog

type ConsoleLogFormatter() = 
    let dateFormat = "yyyy-MM-dd HH:mm:ss"

    let getLevelStr(logLevel: LogLevel) =
        match logLevel with
        | LogLevel.Critical      -> "CRIT"
        | LogLevel.Error         -> "ERRO"
        | LogLevel.Warning       -> "WARN"
        | LogLevel.Informational -> "INFO"
        | LogLevel.Verbose       -> "TRAC"
        | _ -> failwith "getLevelStr"
    
    member this.FormatMessage(logEvent: LogEvent) =
        if logEvent.Level > LogLevel.Informational then
            String.Format("[{0}] {1} - {2} - {3}", getLevelStr(logEvent.Level), logEvent.Timestamp.ToString(dateFormat), logEvent.SourceName, logEvent.Message)
        else
            String.Format("[{0}] {1} - {2}", getLevelStr(logEvent.Level), logEvent.Timestamp.ToString(dateFormat), logEvent.Message)

    interface ITextFormatter with
        member this.FormatMessage(logEvent: LogEvent) =
            this.FormatMessage(logEvent)