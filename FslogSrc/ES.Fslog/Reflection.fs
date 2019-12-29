﻿// Source: http://www.fssnip.net/2V
namespace ES.Fslog

[<AutoOpen>]
module Reflection =

    // [snippet:Implementing dynamic operator]
    open System
    open System.Reflection
    open Microsoft.FSharp.Reflection

    let (?<-) (this : 'Source) (property : string) (value : 'Value) =
        this.GetType().GetProperty(property).SetValue(this, value, null)

    // Various flags that specify what members can be called 
    // NOTE: Remove 'BindingFlags.NonPublic' if you want a version
    // that can call only public methods of classes
    let staticFlags = BindingFlags.NonPublic ||| BindingFlags.Public ||| BindingFlags.Static 
    let instanceFlags = BindingFlags.NonPublic ||| BindingFlags.Public ||| BindingFlags.Instance
    let private ctorFlags = instanceFlags
    let inline asMethodBase(a:#MethodBase) = a :> MethodBase

    let manageArgs (m: MethodBase) (args: Object array) =
        let mutable argsList = []
        let mutable paramsList = []
        let methodParams = m.GetParameters()

        for i=0 to (max methodParams.Length args.Length)-1 do
            let arg =
                if i < args.Length then args.[i]
                else [||] :> Object

            if i < methodParams.Length then
                if methodParams.[i].CustomAttributes |> Seq.exists(fun cad -> cad.AttributeType = typeof<ParamArrayAttribute>) then
                    paramsList <- arg::paramsList
                else argsList <- arg::argsList
            else paramsList <- arg::paramsList
            
        if not paramsList.IsEmpty then
            argsList <- (paramsList |> List.rev |> List.toArray :> Object)::argsList
        argsList |> List.rev |> List.toArray

    let parametersTypeMatches(methodParams: ParameterInfo array, args: Object array) =
      let mutable matches = true
      for i=0 to (min methodParams.Length args.Length) - 1 do
        if 
            not(methodParams.[i].CustomAttributes |> Seq.exists(fun cad -> cad.AttributeType = typeof<ParamArrayAttribute>)) &&
            not(methodParams.[i].ParameterType.IsAssignableFrom(args.[i].GetType())) 
        then matches <- false
      matches

    // The operator takes just instance and a name. Depending on how it is used
    // it either calls method (when 'R is function) or accesses a property
    let (?) (o:obj) name : 'R =
      // The return type is a function, which means that we want to invoke a method
      if FSharpType.IsFunction(typeof<'R>) then
        // Get arguments (from a tuple) and their types
        let argType, resType = FSharpType.GetFunctionElements(typeof<'R>)
        // Construct an F# function as the result (and cast it to the
        // expected function type specified by 'R)
        FSharpValue.MakeFunction(typeof<'R>, fun args ->
      
          // We treat elements of a tuple passed as argument as a list of arguments
          // When the 'o' object is 'System.Type', we call static methods
          let methods, instance, args =               
            let args = 
              // If argument is unit, we treat it as no arguments,
              // if it is not a tuple, we create singleton array,
              // otherwise we get all elements of the tuple
              if argType = typeof<unit> then [||]
              elif not(FSharpType.IsTuple(argType)) then [|args|]
              else FSharpValue.GetTupleFields(args)

            // Static member call (on value of type System.Type)?
            if (typeof<System.Type>).IsAssignableFrom(o.GetType()) then 
              let methods = (unbox<Type> o).GetMethods(staticFlags) |> Array.map asMethodBase
              let ctors = (unbox<Type> o).GetConstructors(ctorFlags) |> Array.map asMethodBase
              Array.concat [ methods; ctors ], null, args
            else 
              o.GetType().GetMethods(instanceFlags) |> Array.map asMethodBase, o, args
        
          // A simple overload resolution based on the name and the number of parameters only
          let methodsDef = 
            [ for m in methods do
                let methodParameters = m.GetParameters()
                if m.Name = name && parametersTypeMatches(methodParameters, args) then
                  let realArgs = (manageArgs m args)
                  if methodParameters.Length = realArgs.Length then yield (m, realArgs) ]
        
          // If we find suitable method or constructor to call, do it!
          match methodsDef with 
          | [] -> failwithf "No method '%s' with %d arguments found" name args.Length
          | _::_::_ -> failwithf "Multiple methods '%s' with %d arguments found" name args.Length
          | [(:? ConstructorInfo as c, args)] -> c.Invoke(args)
          | [ (m, args) ] -> m.Invoke(instance, args) ) |> unbox<'R>

      else
        // The result type is not an F# function, so we're getting a property
        // When the 'o' object is 'System.Type', we access static properties
        let typ, flags, instance = 
          if (typeof<System.Type>).IsAssignableFrom(o.GetType()) 
            then unbox o, staticFlags, null
            else o.GetType(), instanceFlags, o
      
        // Find a property that we can call and get the value
        let prop = typ.GetProperty(name, flags)
        if prop = null && instance = null then 
          // The syntax can be also used to access nested types of a type
          let nested = typ.Assembly.GetType(typ.FullName + "+" + name)
          // Return nested type if we found one
          if nested = null then 
            failwithf "Property or nested type '%s' not found in '%s'." name typ.Name 
          elif not ((typeof<'R>).IsAssignableFrom(typeof<System.Type>)) then
            let rname = (typeof<'R>.Name)
            failwithf "Cannot return nested type '%s' as a type '%s'." nested.Name rname
          else nested |> box |> unbox<'R>
        else
          // Call property and return result if we found some
          let meth = prop.GetGetMethod(true)
          if prop = null then failwithf "Property '%s' found, but doesn't have 'get' method." name
          try meth.Invoke(instance, [| |]) |> unbox<'R>
          with _ -> failwithf "Failed to get value of '%s' property (of type '%s')" name typ.Name   