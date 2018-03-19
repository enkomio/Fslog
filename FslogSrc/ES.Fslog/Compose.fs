namespace ES.Fslog

open System

[<AutoOpen>]
module Compose =

    let log(logName: String) =
        new LogSourceBuilder(logName)

    let verbose (methodName: String) (formatString: String) (logBuilder: LogSourceBuilder) =
        logBuilder.AddLogMethod(methodName, formatString, LogLevel.Verbose)
        logBuilder

    let info (methodName: String) (formatString: String) (logBuilder: LogSourceBuilder) =
        logBuilder.AddLogMethod(methodName, formatString, LogLevel.Informational)
        logBuilder

    let warning (methodName: String) (formatString: String) (logBuilder: LogSourceBuilder) =
        logBuilder.AddLogMethod(methodName, formatString, LogLevel.Warning)
        logBuilder

    let error (methodName: String) (formatString: String) (logBuilder: LogSourceBuilder) =
        logBuilder.AddLogMethod(methodName, formatString, LogLevel.Error)
        logBuilder

    let critical (methodName: String) (formatString: String) (logBuilder: LogSourceBuilder) =
        logBuilder.AddLogMethod(methodName, formatString, LogLevel.Critical)
        logBuilder

    let build(logBuilder: LogSourceBuilder) =
        logBuilder.Build() :?> LogSource

    let buildAndAdd(logProvider: #ILogProvider) (logBuilder: LogSourceBuilder)  =
        let logSource = build(logBuilder)
        logProvider.AddLogSourceToLoggers(logSource)
        logSource