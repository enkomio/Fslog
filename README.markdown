# FSLOG - F# Log

"FSLOG - F# Log" is a simple yet powerful logging framework written in order to be used with a semantic approach in mind.

# Using FSLOG

## On C#

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

## On F#

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

## How to get FSLOG

You can download the source code from [https://github.com/enkomio/Fslog/archive/master.zip](https://github.com/enkomio/Fslog/archive/master.zip).

<div class="row">
  <div class="span1"></div>
  <div class="span6">
    <div class="well well-small" id="nuget">
      FSLOG is also available <a href="http://www.nuget.org/packages/FSharpLog">on NuGet</a>.
      To install the tool, run the following command in the <a href="http://docs.nuget.org/docs/start-here/using-the-package-manager-console">Package Manager Console</a>:
      <pre>PM> Install-Package FSharpLog</pre>
    </div>
  </div>
  <div class="span1"></div>
</div>

## License information

Copyright (C) 2014-2014 Antonio Parata

License: GNU General Public License, version 2 or later; see COPYING.txt included in this archive for details.