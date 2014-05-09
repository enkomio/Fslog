# FSLOG - F# Log

"FSLOG - F# Log" is a simple yet powerful logging framework written in order to be used with a semantic approach in mind.

Simple usage:

C#

Define your logging source:

	internal sealed class StubLogSource : LogSource
    {
        public MyLogger()
            : base("MyLogger")
        { }

        [Log(1, Level = LogLevel.Informational, Message = "Hello {0}")]
        public void SayHelloTo(String name)
        {
            this.WriteLog(1, name);
        }

        [Log(2, Level = LogLevel.Critical, Message = "Pay attention {0} !!!")]
        public void SayPayAttentionTo(String name)
        {
            this.WriteLog(2, name);
        }
    }

Configure your log provider:

	var logProvider = new LogProvider();
    logProvider.AddLogger(new ConsoleLogger(LogLevel.Informational));

Now you can add the log source to the log provider and forget about the last one:
	
	var logSource = new MyLogger();
	logProvider.AddLogSourceToLoggers(logSource);

Call the methods on your log source to start to log messages:
	
	logSource.SayPayAttentionTo("John");
	logSource.SayHelloTo("Michael");

F#

Define your logging source:

	type MyLogger() =
	    inherit LogSource("MyLogger")

	    [<Log(1, Message = "Hello: {0}, Level = LogLevel.Informational)>]
	    member this.SayHelloTo(name: String) =
	        this.WriteLog(1, [|name|])

	    [<Log(2, Message = "Pay attention {0} !!!", Level = LogLevel.Critical)>]
	    member this.SayPayAttentionTo(name: String) =
	        this.WriteLog(2, [|name|])

Configure your log provider:

	let logProvider = new LogProvider()
    logProvider.AddLogger(new ConsoleLogger(LogLevel.Informational))

Now you can add the log source to the log provider and forget about the last one:
	
	let logSource = new MyLogger()
	logProvider.AddLogSourceToLoggers(logSource)

Call the methods on your log source to start to log messages:
	
	logSource.SayPayAttentionTo("John")
	logSource.SayHelloTo("Michael")



## License information

Copyright (C) 2014-2014 Antonio Parata

License: GNU General Public License, version 2 or later; see COPYING.txt included in this archive for details.