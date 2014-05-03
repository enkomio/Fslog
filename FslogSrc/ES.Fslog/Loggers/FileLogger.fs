namespace ES.Fslog.Loggers

open System
open System.Text
open System.IO
open ES.Fslog
open ES.Fslog.TextFormatters

type FileLogger(logLevel: LogLevel, filename: String, textFormatter: ITextFormatter) = 
    inherit BaseLogger()
    
    let _syncRoot = new Object()
    let mutable _fileStream: FileStream option = Some <| File.Open(filename, FileMode.OpenOrCreate, FileAccess.Write, FileShare.ReadWrite)
    
    new(logLevel: LogLevel, filename: String) = new FileLogger(logLevel, filename, new FileLogFormatter())

    member val Level = logLevel with get
    member val FileName = filename with get

    override this.WriteLogEvent(logEvent: LogEvent) =
        if this.EventLogLevelAllowed(logEvent.Level, this.Level) then
            let formattedMessage = textFormatter.FormatMessage(logEvent)
            lock(_syncRoot) (fun () -> 
                let buffer = Encoding.UTF8.GetBytes(formattedMessage + Environment.NewLine)
                _fileStream.Value.Seek(0L, SeekOrigin.End) |> ignore
                _fileStream.Value.Write(buffer, 0, buffer.Length)
                _fileStream.Value.Flush()
            )

    interface IDisposable with        
        member thid.Dispose() =
            lock(_syncRoot) (fun () -> 
                if _fileStream.IsSome then
                 _fileStream.Value.Dispose()
                 _fileStream <- None
            )