namespace ES.Fslog

open System
open System.Collections.Generic

[<AbstractClass>]
type LogSource(name: String) =

    let writeLogEvent (logger: ILogger) (logEvent: LogEvent) =
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
        let logEvent = ref <| LogSourceAnalyzer.retrieveLogSourceInfo(this.Id, logId, args)
        if (!logEvent).IsNone then
            LogSourceAnalyzer.analyzeClass(this, this.Id)
            logEvent := LogSourceAnalyzer.retrieveLogSourceInfo(this.Id, logId, args)
        
        this.Loggers.ForEach(fun logger -> writeLogEvent logger ((!logEvent).Value))