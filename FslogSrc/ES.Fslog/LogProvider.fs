namespace ES.Fslog

open System
open System.Diagnostics
open System.Collections.Generic

type LogProvider() = 

    static let _instance = new Lazy<ILogProvider>(new Func<ILogProvider>(fun () -> upcast new LogProvider()))
    let _loggers = new List<ILogger>()
    let _logSources = new List<LogSource>()

    let updateLoggers() =
        _logSources |> Seq.iter(fun logSource ->
            logSource.Loggers.Clear()
            _loggers |> Seq.iter logSource.AddLogger
        )

    member this.AddLogSourceToLoggers(logSource: LogSource) =
        if not <| _logSources.Contains(logSource) then
            _logSources.Add(logSource)
            updateLoggers()
        
    member this.AddLogger(logger: ILogger) =
        if not <| _loggers.Contains(logger) then
            _loggers.Add(logger)
            updateLoggers()

    static member GetDefault() =
        _instance.Value

    interface ILogProvider with

        member this.AddLogSourceToLoggers(logSource: LogSource) =
            this.AddLogSourceToLoggers(logSource)

        member this.AddLogger(logger: ILogger) =
            this.AddLogger(logger)
