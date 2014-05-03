namespace ES.Fslog

open System

type ILogger = 
    interface
        abstract WriteLogEvent : LogEvent -> unit
    end
    