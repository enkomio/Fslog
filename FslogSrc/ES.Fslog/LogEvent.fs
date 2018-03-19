namespace ES.Fslog

open System

type LogEvent(message: String, level: LogLevel) = 
    member val Id = Guid.NewGuid() with get, set
    member val Message = message with get, set
    member val Level = level with get, set
    member val Timestamp = DateTime.UtcNow with get, set
    member val SourceName = String.Empty with get, set

    override this.ToString() =
        String.Format("[{0}] {1}", this.Level, this.Message)
