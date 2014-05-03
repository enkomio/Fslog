namespace ES.Fslog.TextFormatters

open System
open ES.Fslog

type ITextFormatter = 
    interface
        abstract FormatMessage: LogEvent -> String
    end
