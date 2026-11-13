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
        Dictionary<string, List<string>> Arguments;
        List<string> Positional;
        List<string> ArgIdChars;
        List<Command> RegisteredCommands;
        string Description = "Program has no defined description";
        bool StrictMode = false;
        bool IgnoreCase = false;

        public Args(string[] args, char[] arg_chars = null) :
            this(
                args,
                (arg_chars == null) ? null : arg_chars.Select(c => c.ToString()).ToArray()) { }

        public Args(string[] args, string[] args_strings) {

            ArgIdChars = new List<string>();
            if (args_strings == null)
            {
                args_strings = new string[] { "--", "-", "/" };
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
                StringComparer comparer = IgnoreCase ? StringComparer.OrdinalIgnoreCase : StringComparer.Ordinal;
                Arguments = new Dictionary<string, List<string>>(comparer);
                Positional = new List<string>();
                List<string> values = null;
                bool options_ended = false;
                foreach (var a in RawArguments)
                {
                    if (options_ended)
                    {
                        Positional.Add(a);
                        continue;
                    }

                    if (a == "--")
                    {
                        options_ended = true;
                        continue;
                    }

                    if (ParseCommand(a, out string name, out string value))
                    {
                        values = AddArgument(name, value);
                        continue;
                    }

                    if (values == null)
                        Positional.Add(a);
                    else
                        values.Add(a);
                }

                // check commands (if required)
                foreach (var cmd in RegisteredCommands) {

                    if (!Arguments.ContainsKey(cmd.Name)) {

                        if (cmd.Required)
                            throw new ArgsException($"Required field \"{cmd.Name}\" is not present in arguments");
                    }
                }

                if (StrictMode)
                    CheckStrict();
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

        public void SetStrictMode(bool strict = true) {

            StrictMode = strict;
        }

        public void SetIgnoreCase(bool ignore = true) {

            IgnoreCase = ignore;
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

            if (!Arguments.TryGetValue(id, out List<string> values))
                return default(T);

            // argument without value reads as "1"
            string value = (values.Count == 0) ? "1" : values[0];

            if (typeof(T) == typeof(string))
                return (T)(object)value;

            if (typeof(T) == typeof(bool))
            {

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

        public List<string> GetValues(string id)
        {

            CheckProcessed();

            id = StripArg(id, ArgIdChars.ToArray());

            if (!Arguments.TryGetValue(id, out List<string> values))
                return new List<string>();

            return new List<string>(values);
        }

        public List<string> GetPositional()
        {

            CheckProcessed();

            return new List<string>(Positional);
        }

        private void CheckProcessed()
        {

            if (Arguments == null)
                throw new ArgsException("Arguments were not processed yet, call Process() first");
        }

        private void CheckStrict()
        {

            foreach (var argument in Arguments)
            {

                Command cmd = FindCommand(argument.Key);
                if (cmd == null)
                    throw new ArgsException($"Argument \"{argument.Key}\" is not a known command");

                foreach (var value in argument.Value)
                {

                    if (!cmd.AcceptsValue(value, IgnoreCase))
                        throw new ArgsException($"Argument \"{argument.Key}\" does not accept value \"{value}\"");
                }
            }
        }

        private Command FindCommand(string name)
        {

            StringComparison comparison = IgnoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal;

            return RegisteredCommands.FirstOrDefault(c => string.Equals(c.Name, name, comparison));
        }

        // Returns null when the command is a flag
        private List<string> AddArgument(string name, string value)
        {

            if (StrictMode && Arguments.ContainsKey(name))
                throw new ArgsException($"Argument \"{name}\" is defined more than once");

            if (!Arguments.ContainsKey(name))
                Arguments.Add(name, new List<string>());

            List<string> values = Arguments[name];
            if (value != null)
                values.Add(value);

            Command cmd = FindCommand(name);
            if (cmd != null && cmd.IsFlag)
                return null;

            return values;
        }

        // Value only for the name=value form
        private bool ParseCommand(string token, out string name, out string value)
        {

            name = null;
            value = null;

            if (!IsArg(token, ArgIdChars.ToArray()) || IsNumber(token))
                return false;

            name = StripArg(token, ArgIdChars.ToArray());

            int separator = name.IndexOfAny(new char[] { '=', ':' });
            if (separator >= 0)
            {
                value = name.Substring(separator + 1);
                name = name.Substring(0, separator);
            }

            return name.Length > 0;
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
