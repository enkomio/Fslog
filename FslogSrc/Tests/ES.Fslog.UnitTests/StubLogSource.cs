using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ES.Fslog.UnitTests
{
    internal sealed class StubLogSource : LogSource
    {
        public StubLogSource()
            : base("Log source for testing")
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

        [Log(3, Level = LogLevel.Critical, Message = "ERROR")]
        public void InvalidLogId()
        {
            this.WriteLog(4);
        }
    }
}
