namespace ES.Fslog

open System

type LogEvent(message: String, level: LogLevel) = 
    member val Id = Guid.NewGuid() with get, set
    member val Message = message with get, set
    member val Level = level with get, set
    member val Timestamp = DateTime.Now with get, set
    member val SourceName = String.Empty with get, set
