using System;

namespace Argumentos
{
    public class ArgsException : Exception
    {
        public ArgsException() { }
        public ArgsException(string message) : base(message) { }
        public ArgsException(string message, Exception inner) : base(message, inner) { }
    }

}
