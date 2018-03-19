namespace SmokeTest

open System
open ES.Fslog
open ES.Fslog.UnitTests

module Program =

    [<EntryPoint>]
    let main argv =         
        let version = typeof<ILogProvider>.Assembly.GetName().Version
        Console.WriteLine("Start fluent tests for version {0}", version)
        ``Create a log with one method and one parameter of informational level``()
        ``Create a log with one method and two parameters of warning level``()
        ``Create a log with one method and no parameters of critical level``()
        ``Ensure that if the log level is not enough the message will not be logged``()
        ``A more complex example``()
        ``Fluent creation and adding to logBuilder``()
        ``Ensure that concurrent logging doesn't generate any exception``()
        
        Console.WriteLine("Start unit tests for version {0}", version)
        let test = new LogProviderTests()
        test.Log_a_critical_message()
        test.Log_a_critical_message_by_first_setting_the_log_source()
        test.Ensure_that_informational_message_are_filtered_for_warning_level_logger()
        test.Ensure_that_only_one_instance_of_every_logger_is_added()
        test.Ensure_that_by_adding_logsource_and_logger_we_have_a_consistent_state()
        test.Test_text_writer_logger()
        test.Specify_and_invalid_logId()
        0