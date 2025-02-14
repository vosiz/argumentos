using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Argumentos
{

    public class Args
    {
        List<string> RawArguments;
        Dictionary<string, string> Arguments;
        List<string> ArgIdChars;
        List<Command> RegisteredCommands;
        string Description = "Program has no defined description";

        public Args(string[] args, char[] arg_chars = null) : 
            this(
                args, 
                (arg_chars ?? new char[] { '/', '-' }).Select(c => c.ToString()).ToArray())
        { }

        public Args(string[] args, string[] args_strings) {

            ArgIdChars = new List<string>();
            if (args_strings == null)
            {
                args_strings = new string[] { "-", "/" };
            }
            ArgIdChars.AddRange(args_strings);

            RawArguments = new List<string>();
            foreach (var a in args) {

                // save raw
                RawArguments.Add(a);
            }

            RegisteredCommands = new List<Command>();
        }

        public void AddCommand(Command cmd) {

            try {

                RegisteredCommands.Add(cmd);

            } catch (Exception exc) {

                throw new ArgsException(exc.Message);
            }
        }

        public void Process() {

            try
            {
                // pair arguments and values
                Arguments = new Dictionary<string, string>();
                string last = null;
                foreach (var a in RawArguments)
                {
                    if (last == null) { // argument

                        last = a;
                        continue;
                    }

                    // is argument ID or value
                    if (IsArg(a, ArgIdChars.ToArray()))
                    {
                        // last is not null -> last is argument without value
                        Arguments.Add(
                            StripArg(last, ArgIdChars.ToArray()), "1");
                        last = a;
                    }
                    else { // value

                        Arguments.Add(
                            StripArg(last, ArgIdChars.ToArray()), a);
                        last = null;
                    }
                }

                if (last != null) {

                    // last argument was not processed (single element, no value)
                    Arguments.Add(
                        StripArg(last, ArgIdChars.ToArray()), "1");
                }

                // check commands (if required)
                foreach (var cmd in RegisteredCommands) {

                    if (!Arguments.ContainsKey(cmd.Name)) {

                        if (cmd.Required)
                            throw new ArgsException($"Required field \"{cmd.Name}\" is not present in arguments");
                    }
                }

            }
            catch (ArgsException exc)
            {
                throw exc;
            }
            catch (Exception exc)
            {
                throw new ArgsException(exc.Message);
            }
        }

        public void AddDescription(string desc) {

            Description = desc;
        }

        public string PrintHelp() {

            string output = NewRow("This is auto-generated HELP");
            output += NewRow(Description);
            output += NewRow("");
            output += NewRow("Recognized arguments");

            foreach (var cmd in RegisteredCommands) {

                output += cmd.PrintHelp();
            }


            return output;
        }

        public bool HasArgument(string id) {

            id = StripArg(id, ArgIdChars.ToArray());

            return Arguments.ContainsKey(id);
        }

        public T GetArgument<T>(string id) where T : class
        {
            if (Arguments.TryGetValue(id, out string value))
            {
                try
                {
                    if (typeof(T) == typeof(bool) && bool.TryParse(value, out bool b))
                        return (T)(object)b;

                    if (typeof(T) == typeof(int) && int.TryParse(value, out int i))
                        return (T)(object)i;

                    if (typeof(T) == typeof(float) && float.TryParse(value, out float f))
                        return (T)(object)f;

                    return (T)(object)value; // string
                }
                catch
                {
                    return null; // error in conversion?
                }
            }

            return null;
        }

        private bool IsArg(string argument, string[] arg_chars)
        {

            foreach (var s in arg_chars)
            {

                if (argument.StartsWith(s))
                    return true;
            }

            return false;
        }

        private string StripArg(string argument, string[] arg_chars)
        {

            foreach (var s in arg_chars)
            {

                argument = argument.Replace(s, "");
            }

            return argument;
        }

        public static string NewRow(string row) {

            return row + Environment.NewLine;
        }
    }
}
