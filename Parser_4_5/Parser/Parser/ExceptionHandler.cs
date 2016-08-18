using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parser
{

    public delegate void ExceptionHandler(object sender,ExceptionArgs exception);
  public   class ExceptionArgs:EventArgs
    {
        public string Message { get; set; }
        public string Function { get; set; }

        public ExceptionArgs()
        {

        }

        public ExceptionArgs(string message, string function = "")
        {
            this.Message = message;
            this.Function = function;
        }
    }
}
