namespace ES.Fslog.Loggers

open System
open ES.Fslog

[<AbstractClass>]
type BaseLogger() = 
        
    abstract EventLogLevelAllowed: LogLevel * LogLevel -> Boolean
    default this.EventLogLevelAllowed(logEventLevel: LogLevel, loggerLevel: LogLevel) =
        logEventLevel >= loggerLevel

    abstract WriteLogEvent: LogEvent ->unit    
    abstract Level: LogLevel
    
    interface ILogger with
        member this.WriteLogEvent(logEvent: LogEvent) =
            this.WriteLogEvent(logEvent)

        member this.Level  
            with get() = this.Level