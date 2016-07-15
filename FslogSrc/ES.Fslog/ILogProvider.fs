namespace ES.Fslog

open System

type ILogProvider = 
    interface
        inherit IDisposable
        abstract Id : Guid
        abstract AddLogSourceToLoggers : LogSource -> unit
        abstract AddLogger : ILogger -> unit
        abstract GetSources : unit -> LogSource array
        abstract GetLoggers : unit -> ILogger array
    end
