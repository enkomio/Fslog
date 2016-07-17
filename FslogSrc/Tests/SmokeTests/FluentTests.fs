namespace SmokeTest

open System
open ES.Fslog
open ES.Fslog.UnitTests
open System.Reflection
open Microsoft.FSharp.Reflection

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