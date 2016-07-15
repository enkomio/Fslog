namespace ES.Fslog

open System
open System.Linq
open System.Collections.Generic
open LogSourceAnalyzer

[<AbstractClass>]
type LogSource(name: String) =

    let writeLogEvent (logEvent: LogEvent) (logger: ILogger) =
        logEvent.SourceName <- name
        logger.WriteLogEvent(logEvent)

    member val Id = Guid.NewGuid() with get, set
    member val Loggers = new List<ILogger>() with get
    member val Name = name with get

    abstract AddLogger: ILogger -> unit
    default this.AddLogger(logger: ILogger) =
        this.Loggers.Add(logger)

    abstract WriteLog: Int32 * Object array -> unit
    default this.WriteLog(logId: Int32, [<ParamArray>] args: Object array) =
        let logEvent = ref <| retrieveLogSourceInfo(this.Id, logId, args)
        if (!logEvent).IsNone then
            LogSourceAnalyzer.analyzeClass(this, this.Id)
            logEvent := retrieveLogSourceInfo(this.Id, logId, args)
        
        this.Loggers.ToList()
        |> Seq.iter(writeLogEvent ((!logEvent).Value))