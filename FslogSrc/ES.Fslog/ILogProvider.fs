namespace ES.Fslog

open System

type ILogProvider = 
    interface
        inherit IDisposable
        abstract AddLogSourceToLoggers : LogSource -> unit
        abstract AddLogger : ILogger -> unit
    end
