using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArgsTesting
{
    class Program
    {
        static void Main(string[] args)
        {

            try
            {
                // my scenario
                args = new string[] { 
                
                    "-input",
                    "somefile.txt",
                    "debug"
                };

                // Basic use
                // create primary instnace
                var ar = new Argumentos.Args(args);

                // create recognized command in arguments
                var cmd_input_file = new Argumentos.Command("input", "Input file path");

                // add command to main instance
                ar.AddCommand(cmd_input_file);

                // ready to process args
                ar.Process();

                // get value by key
                var value = ar.GetArgument<string>("input");
                if (value == null)
                    throw new Exception("Input was not defined");
                else Console.WriteLine("... processing file " + value);



                //// Advanced usage
                //// create instance with recognizing desired argument prefixes
                //var ar = new Argumentos.Args(args, new string[] { "-", "--"});

                //// add your own description 
                //ar.AddDescription("This programs serves humanity and robots too!");

                //// create a required command - argumewnt which has to be in command line
                //var cmd_input_file = new Argumentos.Command("input", "Input file path", true);
                //ar.AddCommand(cmd_input_file);
                //// optional command
                //var cmd_debug = new Argumentos.Command("debug", "Use to debug");
                //ar.AddCommand(cmd_debug);
                //// optional command - with optons documented
                //var cmd_log_level = new Argumentos.Command("log", "Use to log program actions to a file");
                //cmd_log_level.AddArgumentValueDesc("warning", "Log only warning and up events");
                //cmd_log_level.AddArgumentValueDesc("error", "Log only errors");
                //cmd_log_level.AddArgumentValueDesc("(default) normal", "Enables logging at normal level");
                //ar.AddCommand(cmd_log_level);

                ////ready to process args
                //ar.Process();
                //// if "input" argument was not set (required) it will entirely fail with exception

                //// check if argument is set, you can check with prefixes
                //if (ar.HasArgument("--debug"))
                //    Console.WriteLine("Debug has been set!");
                //// non-pair arguments has always ther value set to "1"

                //// get value of optional
                //var value = ar.GetArgument<string>("log");

                //// print help
                //if (ar.HasArgument("-help"))
                //{
                //    // your code / your own help screen
                //}
                //else {

                //    var output = ar.PrintHelp();
                //    Console.WriteLine(output);
                //}

                //// if argument of "help" is not defined, it will autocreate help-like description to output

            }
            catch (Exception exc) {

                throw exc;            
            }

        }
    }
}
