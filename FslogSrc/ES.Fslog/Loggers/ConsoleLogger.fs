namespace ES.Fslog.Loggers

open System
open ES.Fslog
open ES.Fslog.TextFormatters

type ConsoleLogger(logLevel: LogLevel, textFormatter: ITextFormatter) = 
    inherit BaseLogger()
    
    let _syncRoot = new Object()

    let writeWithColor(text: String, color: ConsoleColor) =
        lock(_syncRoot) (fun () -> 
            let old = Console.ForegroundColor
            Console.ForegroundColor <- color;
            Console.WriteLine(text)
            Console.ForegroundColor <- old
        )

    new(logLevel: LogLevel) = new ConsoleLogger(logLevel, new ConsoleLogFormatter())

    default val Level = logLevel with get

    override this.WriteLogEvent(logEvent: LogEvent) =
        if this.EventLogLevelAllowed(logEvent.Level, this.Level) then
            let formattedMessage = textFormatter.FormatMessage(logEvent)
            let color = 
                match logEvent.Level with
                | LogLevel.Verbose -> ConsoleColor.Green
                | LogLevel.Informational -> Console.ForegroundColor
                | LogLevel.Warning -> ConsoleColor.Yellow
                | LogLevel.Error -> ConsoleColor.Magenta
                | LogLevel.Critical -> ConsoleColor.Red
                | _ -> raise <| new ApplicationException("The log level " + logEvent.Level.ToString() + " isn't a valid one")

            writeWithColor(formattedMessage, color)