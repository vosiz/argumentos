using System;
using System.Collections.Generic;
using System.Globalization;
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
                    if (IsArg(a, ArgIdChars.ToArray()) && !IsNumber(a))
                    {
                        // last is not null -> last is argument without value
                        AddArgument(last, "1");
                        last = a;
                    }
                    else { // value

                        AddArgument(last, a);
                        last = null;
                    }
                }

                if (last != null) {

                    // last argument was not processed (single element, no value)
                    AddArgument(last, "1");
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

            CheckProcessed();

            id = StripArg(id, ArgIdChars.ToArray());

            return Arguments.ContainsKey(id);
        }

        public T GetArgument<T>(string id)
        {

            CheckProcessed();

            id = StripArg(id, ArgIdChars.ToArray());

            if (!Arguments.TryGetValue(id, out string value))
                return default(T);

            if (typeof(T) == typeof(string))
                return (T)(object)value;

            if (typeof(T) == typeof(bool))
            {

                // argument without value is stored as "1"
                if (value == "1")
                    return (T)(object)true;

                if (value == "0")
                    return (T)(object)false;

                if (bool.TryParse(value, out bool b))
                    return (T)(object)b;
            }

            if (typeof(T) == typeof(int) && int.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out int i))
                return (T)(object)i;

            if (typeof(T) == typeof(float) && float.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out float f))
                return (T)(object)f;

            throw new ArgsException($"Argument \"{id}\" has value \"{value}\" which cannot be read as {typeof(T).Name}");
        }

        private void CheckProcessed()
        {

            if (Arguments == null)
                throw new ArgsException("Arguments were not processed yet, call Process() first");
        }

        private void AddArgument(string argument, string value)
        {

            string name = StripArg(argument, ArgIdChars.ToArray());

            if (Arguments.ContainsKey(name))
                throw new ArgsException($"Argument \"{name}\" is defined more than once");

            Arguments.Add(name, value);
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

        private bool IsNumber(string argument)
        {

            return double.TryParse(argument, NumberStyles.Float, CultureInfo.InvariantCulture, out double number);
        }

        private string StripArg(string argument, string[] arg_chars)
        {

            bool stripped = true;
            while (stripped)
            {

                stripped = false;
                foreach (var s in arg_chars)
                {

                    if (s.Length > 0 && argument.StartsWith(s, StringComparison.Ordinal))
                    {

                        argument = argument.Substring(s.Length);
                        stripped = true;
                    }
                }
            }

            return argument;
        }

        public static string NewRow(string row) {

            return row + Environment.NewLine;
        }
    }
}
