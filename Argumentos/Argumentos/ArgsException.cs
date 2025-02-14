using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Argumentos
{
    public class ArgsException : Exception
    {
        public ArgsException(string message) : base("ArgsExc: " + message) { }
    }

}
