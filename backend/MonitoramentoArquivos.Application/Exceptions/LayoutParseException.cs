using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System;


namespace MonitoramentoArquivos.Application.Exceptions
{
    public class LayoutParseException : Exception
    {
        public LayoutParseException(string message) : base(message) { }
    }
}
