namespace ES.Fslog

open System
open System.Linq
open System.Collections.Generic
open System.Reflection
open System.Reflection.Emit
open LogSourceAnalyzer

type LogSourceBuilder(name: String) =    
    let _assemblyName = new AssemblyName(name + "LoggerAssembly")
    let _assemblyBuilder = AppDomain.CurrentDomain.DefineDynamicAssembly(_assemblyName, AssemblyBuilderAccess.Run)
    let _moduleBuilder = _assemblyBuilder.DefineDynamicModule(_assemblyName.Name)
    let _typeBuilder = _moduleBuilder.DefineType(name + "Logger", TypeAttributes.NotPublic, typeof<LogSource>)
    let _typeConstructor = _typeBuilder.DefineConstructor(MethodAttributes.Public, CallingConventions.Standard, [|typeof<String>|])
    let _attributes = new List<String * Int32 * CustomAttributeBuilder>()
    let mutable _index = 0

    do        
        // invoke the base constructor
        let baseConstructor = typeof<LogSource>.GetConstructor([|typeof<String>|])
        let ilGenerator = _typeConstructor.GetILGenerator()
        ilGenerator.Emit(OpCodes.Ldarg_0)
        ilGenerator.Emit(OpCodes.Ldarg_1)
        ilGenerator.Emit(OpCodes.Call, baseConstructor)
        ilGenerator.Emit(OpCodes.Ret)

    let getIndex() =
        _index <- _index + 1
        _index

    member internal this.Attributes with get() = _attributes

    member internal this.AddLogMethod(methodName: String, formatString: String, logLevel: LogLevel) =   
        let index = getIndex()
        let constructorInfo = typeof<LogAttribute>.GetConstructor([|typeof<Int32>|])        
        let messageProperty = typeof<LogAttribute>.GetProperty("Message")
        let levelProperty = typeof<LogAttribute>.GetProperty("Level")
        let attributeBuilder = new CustomAttributeBuilder(constructorInfo, [|box index|], [|messageProperty; levelProperty|], [|formatString :> Object; logLevel :> Object|])        
        this.Attributes.Add(methodName, index, attributeBuilder)     
        
    member internal this.Build() =
        this.Attributes
        |> Seq.iter(fun (methodName, index, attribute) ->
            let writeLogMethod = typeof<LogSource>.GetMethod("WriteLog")
            let methodBuilder = _typeBuilder.DefineMethod(methodName, MethodAttributes.Public, CallingConventions.Standard, null, [|typeof<Object array>|])
            methodBuilder.SetCustomAttribute(attribute)
            
            // set the ParameterAttributes to the argument
            let parameterBuilder = methodBuilder.DefineParameter(1, ParameterAttributes.None, "args")
            let constructorInfo = typeof<ParamArrayAttribute>.GetConstructor([||])        
            let attributeBuilder = new CustomAttributeBuilder(constructorInfo, [||])
            parameterBuilder.SetCustomAttribute(attributeBuilder)
            
            let ilGenerator = methodBuilder.GetILGenerator()
            
            // push this
            ilGenerator.Emit(OpCodes.Ldarg_0)

            // push logId
            ilGenerator.Emit(OpCodes.Ldc_I4, index)

            // push format string parameters
            ilGenerator.Emit(OpCodes.Ldarg_1)
            
            ilGenerator.Emit(OpCodes.Call, writeLogMethod)
            ilGenerator.Emit(OpCodes.Ret)
        )

        let loggerType = _typeBuilder.CreateType()
        Activator.CreateInstance(loggerType, [|name :> Object|])