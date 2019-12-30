namespace ES.Fslog

open System
open System.Collections.Generic

type LogProvider() = 
    static let _instance = new Lazy<ILogProvider>(new Func<ILogProvider>(fun () -> upcast new LogProvider()))
    static let _logProviders = new Dictionary<Guid, ILogProvider>()

    let _syncRoot = new Object()
    let _loggers = new List<ILogger>()
    let _logSources = new List<LogSource>()

    let updateLoggers() =
        _logSources |> Seq.iter(fun logSource ->
            logSource.Loggers.Clear()
            _loggers |> Seq.iter logSource.AddLogger
        )

    member this.Id = Guid.NewGuid()

    member this.AddLogSourceToLoggers(logSource: LogSource) =
        lock _syncRoot (fun () ->
            if not <| _logSources.Contains(logSource) then
                _logSources.Add(logSource)
                updateLoggers()
        )        
        
    member this.AddLogger(logger: ILogger) =
        lock _syncRoot (fun () ->
            if not <| _loggers.Contains(logger) then
                _loggers.Add(logger)
                updateLoggers()
        )

    static member GetDefault() =
        _instance.Value

    static member AddGlobal(logProvider: ILogProvider) =
        _logProviders.[logProvider.Id]

    static member Get(logProviderId: Guid) =
        _logProviders.[logProviderId]

    member this.Dispose() =
        _loggers
        |> Seq.iter(function
            | :? IDisposable as disposable -> disposable.Dispose()
            | _ -> ()
        )

    member this.GetSources() =
        _logSources |> Seq.toArray

    member this.GetLoggers() =
        _loggers |> Seq.toArray

    interface ILogProvider with

        member this.Id = this.Id

        member this.AddLogSourceToLoggers(logSource: LogSource) =
            this.AddLogSourceToLoggers(logSource)

        member this.AddLogger(logger: ILogger) =
            this.AddLogger(logger)

        member this.GetSources() =
            this.GetSources()

        member this.GetLoggers() =
            this.GetLoggers()

        member this.Dispose() =
            this.Dispose()
