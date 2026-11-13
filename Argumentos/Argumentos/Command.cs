using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        public string PrintHelp() {

            var output = Args.NewRow(
                string.Format("- {2}{0} - {1}", 
                Name, Description, Required ? "" : "(optional) ")
            );

            if (ArgsDesc.Count > 0) {

                // i know... i am lazy to look up the formating...
                output += Args.NewRow("     Recognized values:");
                foreach (var argsdesc in ArgsDesc) {

                    output += Args.NewRow(string.Format("     - {0} -> {1}", argsdesc.Key, argsdesc.Value));
                }
            }

            return output;
        }
    }
}
