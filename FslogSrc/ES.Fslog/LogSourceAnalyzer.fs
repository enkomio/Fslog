namespace ES.Fslog

open System
open System.Linq
open System.Collections.Generic
open System.Reflection

module internal LogSourceAnalyzer = 
    let private _syncRoot = new Object()
    let private _logEventDescriptors = new Dictionary<Guid, Dictionary<Int32, LogAttribute>>()

    let private createLogEvent(logAttribute: LogAttribute, args: Object array) =
        let formattedMessage = String.Format(logAttribute.Message, args)
        new LogEvent(formattedMessage, logAttribute.Level)

    let retrieveLogSourceInfo(logSourceId: Guid, id: Int32, args: Object array) =         
        if _logEventDescriptors.ContainsKey(logSourceId) then
            let logSourceDecriptor = _logEventDescriptors.[logSourceId]
            if (logSourceDecriptor.ContainsKey(id)) then
                let logAttribute = logSourceDecriptor.[id]
                Some <| createLogEvent(logAttribute, args)
            else
                raise <| new ApplicationException("Unable to found the log method with attribute id: " + string id)
        else
            None

    let analyzeClass(classObj: Object, logSourceId: Guid) =
        lock(_syncRoot) (fun () -> 
            if not <| _logEventDescriptors.ContainsKey(logSourceId) then
                let classAttributes = new Dictionary<Int32, LogAttribute>()
                _logEventDescriptors.Add(logSourceId, classAttributes)

                for methodInfo in classObj.GetType().GetMethods(BindingFlags.Public ||| BindingFlags.NonPublic ||| BindingFlags.Instance) do
                    let attributes = methodInfo.GetCustomAttributes<LogAttribute>()
                    if attributes.Any() then
                        attributes
                        |> Seq.head
                        |> (fun logAttribute -> classAttributes.Add(logAttribute.Id, logAttribute))
            )
