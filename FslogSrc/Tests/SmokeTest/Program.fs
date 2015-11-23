namespace SmokeTest

open System
open ES.Fslog
open ES.Fslog.UnitTests

module Program =

    [<EntryPoint>]
    let main argv =         
        let version = typeof<ILogProvider>.Assembly.GetName().Version
        Console.WriteLine("Start smoke test for version {0}", version)
        let test = new LogProviderTests()
        test.Log_a_critical_message()
        test.Log_a_critical_message_by_first_setting_the_log_source()
        test.Ensure_that_informational_message_are_filtered_for_warning_level_logger()
        test.Ensure_that_only_one_instance_of_every_logger_is_added()
        test.Ensure_that_by_adding_logsource_and_logger_we_have_a_consistent_state()
        0