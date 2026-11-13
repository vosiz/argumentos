using System;
using System.Collections.Generic;
using System.Linq;

namespace Argumentos
{
    public class Command
    {

        public string Name { get; private set; }
        public bool Required { get; private set; }
        public bool IsFlag { get; private set; }

        string Description;
        Dictionary<string, string> ArgsDesc;

        public Command(string name, string description, bool required = false) {

            ArgsDesc = new Dictionary<string, string>();
            Name = name;
            Description = description;
            Required = required;
        }

        public void AddArgumentValueDesc(string argument, string desc) {

            if (ArgsDesc.ContainsKey(argument))
                throw new ArgsException($"Value \"{argument}\" is already documented for command \"{Name}\"");

            ArgsDesc.Add(argument, desc);
        }

        public void SetFlag(bool flag = true) {

            IsFlag = flag;
        }

        public bool AcceptsValue(string value, bool ignore_case = false) {

            if (ArgsDesc.Count == 0)
                return true;

            StringComparison comparison = ignore_case ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal;

            return ArgsDesc.Keys.Any(k => string.Equals(k, value, comparison));
        }

        public string GetHelp(string shown_name = null) {

            string output =
                string.Format("  {0}{1} - {2}{3}", shown_name ?? Name, Required ? "" : " (optional)", Description, Environment.NewLine);

            if (ArgsDesc.Count > 0) {

                output += "      Recognized values:" + Environment.NewLine;
                foreach (var argsdesc in ArgsDesc) {

                    output += string.Format("      - {0} -> {1}{2}", argsdesc.Key, argsdesc.Value, Environment.NewLine);
                }
            }

            return output;
        }
    }
}
