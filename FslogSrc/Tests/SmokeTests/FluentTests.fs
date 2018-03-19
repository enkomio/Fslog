namespace SmokeTest

open System
open System.Collections.Generic
open System.Threading.Tasks
open System.Reflection
open Microsoft.FSharp.Reflection
open ES.Fslog
open ES.Fslog.UnitTests

[<AutoOpen>]
module FluentTests =

    let getMockLogger(sut: LogSource) =
        let lp = new LogProvider()
        let mockLogger = new MockLogger(Level = LogLevel.Informational)
        lp.AddLogger(mockLogger)
        lp.AddLogSourceToLoggers(sut)
        mockLogger

    let ``Create a log with one method and one parameter of informational level``() =        
        let sut = 
            log "FluentTest1"
            |> info "SayHello" "Hello: {0}!"
            |> warning "Warn" "{0} be careful!! {1}"
            |> build

        let ml = new MockLogger(Level = LogLevel.Informational)
        sut?AddLogger(ml)

        sut?SayHello(sut?Name)
        
        assert(ml.LastLoggedEvent.Message.Equals("Hello: FluentTest1!", StringComparison.Ordinal))

    let ``Create a log with one method and two parameters of warning level``() =        
        let sut = 
            log "FluentTest2"
            |> warning "Warn" "{0} be careful!! {1}"
            |> build
        let ml = getMockLogger(sut)

        sut?Warn("World", ";)")

        assert(ml.LastLoggedEvent.Message.Equals("World be careful!! ;)", StringComparison.Ordinal))

    let ``Create a log with one method and no parameters of critical level``() =        
        let sut = 
            log "FluentTest3"
            |> critical "IsThisTheEndOfTheWorldAsWeKnowIt" "Asta la vista amigos!"
            |> build
        let ml = getMockLogger(sut)

        sut?IsThisTheEndOfTheWorldAsWeKnowIt()

        assert(ml.LastLoggedEvent.Message.Equals("Asta la vista amigos!", StringComparison.Ordinal))

    let ``Ensure that if the log level is not enough the message will not be logged``() =        
        let sut = 
            log "FluentTest4"
            |> verbose "Warn" "{0} be careful!! {1}"
            |> build
        let ml = getMockLogger(sut)

        sut?Warn("World", ";)")

        assert(Object.Equals(ml.LastLoggedEvent, null))

    let ``A more complex example``() =        
        let sut = 
            log "FluentTest5"
            |> verbose "NoPrint" "I will not be printed :("
            |> info "Start" "Process started!"
            |> warning "FileNotFound" "Unable to open the file {0}, create it"
            |> warning "DirectoryNotFound" "Unable to list file in directory: {0}. Create it."
            |> error "UnableToCreateFile" "Unable to create the file: {0} in directory: {1}"
            |> critical "DatabaseDown" "Database is not reachable, this is not good!"
            |> build
        let ml = getMockLogger(sut)

        sut?NoPrint()
        sut?Start()
        sut?FileNotFound("log.txt")
        sut?DirectoryNotFound("logDirectory/")
        sut?UnableToCreateFile("log.txt", "logDirectory/")
        sut?DatabaseDown()

        assert(ml.NumOfLoggedEvents = 5)
        assert(ml.ContainsLogMessage("Process started!"))
        assert(ml.ContainsLogMessage("Unable to open the file log.txt, create it"))
        assert(ml.ContainsLogMessage("Unable to list file in directory: logDirectory/. Create it."))
        assert(ml.ContainsLogMessage("Unable to create the file: log.txt in directory: logDirectory/"))
        assert(ml.ContainsLogMessage("Database is not reachable, this is not good!"))
        assert(not(ml.ContainsLogMessage("I will not be printed :(")))
        assert(ml.LastLoggedEvent.Message.Equals("Database is not reachable, this is not good!", StringComparison.Ordinal))

    let ``Fluent creation and adding to logBuilder``() =
        let lp = new LogProvider()
        let mockLogger = new MockLogger(Level = LogLevel.Informational)
        lp.AddLogger(mockLogger)

        let sut =
            log "FluentTest6"
            |> info "Log" "Hello world from: {0}"
            |> buildAndAdd lp
                    
        sut?Log("Antonio")
        
        assert(mockLogger.ContainsLogMessage("Hello world from: Antonio"))

    let ``Ensure that concurrent logging doesn't generate any exception``() =
        let rnd = new Random()

        let lp = new LogProvider()
        let ml = new MockLogger(Level = LogLevel.Informational)
        lp.AddLogger(ml)

        // shuffle, src: http://www.fssnip.net/L/title/Array-shuffle
        let swap (a: _[]) x y =
            let tmp = a.[x]
            a.[x] <- a.[y]
            a.[y] <- tmp

        let shuffle a =
            Array.iteri (fun i _ -> swap a i (rnd.Next(i, Array.length a))) a

        let sut = 
            log "FluentTest7"
            |> verbose "NoPrint" "I will not be printed :("
            |> info "Start" "Process started!"
            |> warning "FileNotFound" "Unable to open the file {0}, create it"
            |> warning "DirectoryNotFound" "Unable to list file in directory: {0}. Create it."
            |> error "UnableToCreateFile" "Unable to create the file: {0} in directory: {1}"
            |> critical "DatabaseDown" "Database is not reachable, this is not good!"
            |> buildAndAdd lp

        let callbacks = [|
            fun () -> sut?NoPrint()
            fun () -> sut?Start()
            fun () -> sut?FileNotFound("log.txt")
            fun () -> sut?DirectoryNotFound("logDirectory/")
            fun () -> sut?UnableToCreateFile("log.txt", "logDirectory/")
            fun () -> sut?DatabaseDown()
        |]
                
        // create callbacks to invoke        
        let allCallbacks = new List<unit -> unit>()        
        for i=0 to rnd.Next(50, 100) do            
            shuffle callbacks
            callbacks |> Seq.iter(fun callback -> 
                allCallbacks.Add(callback)
            )

        // invoke callbacks
        Parallel.ForEach(allCallbacks, fun callback -> callback()) |> ignore

        assert(ml.ContainsLogMessage("Process started!"))
        assert(ml.ContainsLogMessage("Unable to open the file log.txt, create it"))
        assert(ml.ContainsLogMessage("Unable to list file in directory: logDirectory/. Create it."))
        assert(ml.ContainsLogMessage("Unable to create the file: log.txt in directory: logDirectory/"))
        assert(ml.ContainsLogMessage("Database is not reachable, this is not good!"))
        assert(not(ml.ContainsLogMessage("I will not be printed :(")))
        