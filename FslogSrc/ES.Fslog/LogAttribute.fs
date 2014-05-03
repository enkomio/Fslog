namespace ES.Fslog

open System

[<AttributeUsage(AttributeTargets.Method)>]
type LogAttribute(id: Int32) = 
    inherit Attribute()

    member val Id = id with get
    member val Message = String.Empty with get, set
    member val Level = LogLevel.Informational with get, set