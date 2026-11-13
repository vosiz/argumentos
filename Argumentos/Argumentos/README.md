# Argumentos
Small .NET Standard 2.0 library for reading command-line arguments — works in .NET Framework 4.6.1+ and .NET Core / .NET 5+.

Pairs commands with their values, checks the required ones, can refuse anything it does not know and builds a help screen.

## Features
- `--command`, `-command` and `/command` out of the box, or own prefixes
- A command with any number of values after it, or `--log=warning` / `/log:warning`
- Flags, positional arguments and `--` as the end of options
- Required commands and documented values; add, remove or replace commands
- Optional strict mode (throws on unknown commands and values) and case-insensitive names
- Typed reads (string, bool, int, long, float, double) and a generated help screen

## Basic example
```csharp
var ar = new Argumentos.Args(args);

var cmd_log = new Argumentos.Command("log", "Logging level");
cmd_log.AddArgumentValueDesc("warning", "Log only warning and up events");
cmd_log.AddArgumentValueDesc("error", "Log only errors");
ar.AddCommand(cmd_log);

var cmd_verbose = new Argumentos.Command("verbose", "Prints more details");
cmd_verbose.SetFlag();
ar.AddCommand(cmd_verbose);

ar.SetStrictMode();
ar.Process();

string log = ar.GetArgument<string>("log");
bool verbose = ar.GetArgument<bool>("verbose");
List<string> files = ar.GetPositional();
```

Full documentation and sources at [GitHub](https://github.com/vosiz/argumentos).
