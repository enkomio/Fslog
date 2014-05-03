namespace ES.Fslog

open System
open System.Collections.Generic

type LogProvider() = 

    static let _instance = new Lazy<ILogProvider>(new Func<ILogProvider>(fun () -> new LogProvider() :> ILogProvider))
    let _loggers = new List<ILogger>()

    member this.AddLogSourceToLoggers(logSource: LogSource) =
        _loggers
        |> Seq.iter logSource.AddLogger

    member this.AddLogger(logger: ILogger) =
        _loggers.Add(logger)

    static member GetDefault() =
        _instance.Value
    
    interface ILogProvider with

        member this.AddLogSourceToLoggers(logSource: LogSource) =
            this.AddLogSourceToLoggers(logSource)

        member this.AddLogger(logger: ILogger) =
            this.AddLogger(logger)
