namespace ES.Fslog

open System

type ILogProvider = 
    interface
        abstract AddLogSourceToLoggers : LogSource -> unit
        abstract AddLogger : ILogger -> unit
    end
