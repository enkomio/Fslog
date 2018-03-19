namespace ES.Fslog

open System
open System.Linq
open System.Collections.Generic
open System.Collections.Concurrent
open System.Reflection

module internal LogSourceAnalyzer = 
    let private _syncRoot = new Object()
    let private _logEventDescriptors2 = new ConcurrentDictionary<Guid, ConcurrentDictionary<Int32, LogAttribute>>()

    let private _analyzedClasses = new HashSet<Object>()
    let private _logEventDescriptors = new ConcurrentDictionary<String, LogAttribute>()

    let private createLogEvent(logAttribute: LogAttribute, args: Object array) =
        let formattedMessage = String.Format(logAttribute.Message, args)
        new LogEvent(formattedMessage, logAttribute.Level)
        
    let private getEventId(logSourceId: Guid, logId: Int32) =
        String.Format("{0}_{1}", logSourceId, logId)

    let private analyzeClass(classObj: Object, logSourceId: Guid) =
        classObj.GetType().GetMethods(BindingFlags.Public ||| BindingFlags.NonPublic ||| BindingFlags.Instance)
        |> Seq.iter(fun methodInfo ->
            match methodInfo.GetCustomAttributes<LogAttribute>() |> Seq.tryHead with
            | Some logAttribute ->
                let eventId = getEventId(logSourceId, logAttribute.Id)
                _logEventDescriptors.[eventId] <- logAttribute
            | None -> ()
        )

    let rec getLogEvent(classObj: Object, logSourceId: Guid, logId: Int32, args: Object array) =
        let eventId = getEventId(logSourceId, logId)
        let logAttribute = ref(new LogAttribute(-1))
        if _logEventDescriptors.TryGetValue(eventId, logAttribute) then            
            createLogEvent(!logAttribute, args)
        else
            // This specific log event was found, try to analyze the class
            lock(_syncRoot) (fun () -> 
                if _logEventDescriptors.TryGetValue(eventId, logAttribute) then            
                    createLogEvent(!logAttribute, args)
                elif _analyzedClasses.Add(classObj) then
                    analyzeClass(classObj, logSourceId)
                    getLogEvent(classObj, logSourceId, logId, args)
                else
                    // the class was already analyzed and the method not found, raise an error
                    raise <| new ApplicationException("Unable to found the log method with attribute id: " + string logId)
            )