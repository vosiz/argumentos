# Argumentos
Small library to parse and process arguments from command-line

## Motivation
Every console or form app ends up reading its command-line arguments by hand sooner or later. Argumentos does the boring part once — pairs commands with their values, checks the required ones, can refuse anything it does not know and puts together a help screen — so it does not have to be written again in each C#/.Net solution.

MIT — use it however you like.

## Requirements
- .NET Standard 2.0 — .NET Framework 4.6.1 and newer, .NET Core 2.0 and newer, .NET 5 and newer

## Bug tracker
No tracked

## Features
- `--command`, `-command` and `/command` out of the box, or own prefixes
- A command with any number of values after it, or a value through `--log=warning` / `/log:warning`
- Flags, positional arguments and `--` as the end of options
- Commands — recognized arguments with description, required ones and documented values; add, remove or replace them
- Optional strict mode that throws on unknown commands and values, optional case-insensitive names
- Typed reads of values and a generated help screen

See [Roadmap](#roadmap) for full details.

## Roadmap
- [x] Pairing of arguments and values
- [x] Default prefixes `--`, `-` and `/`
- [x] Own argument prefixes
- [x] Several values after one command
- [x] Value through a separator (`=`, `:`)
- [x] Flags
- [x] Positional arguments and `--`
- [x] Commands (recognized arguments with description)
- [x] Removing and replacing commands
- [x] Required arguments
- [x] Documented argument values
- [x] Strict mode
- [x] Case-insensitive names
- [x] Typed reads (string, bool, int, long, float, double)
- [x] Help screen

## Test projects
The solution includes one extra project, not published as a package, used during development:
- `Tests` — reflection-based test runner exercising the library

## Setup
Install from NuGet:
```
dotnet add package Argumentos
```
Or search for "Vosiz" or "Argumentos" in Visual Studio nuget package manager.

## Manual
See [Test projects](#test-projects) above for more usage examples.

Include **Argumentos** by using or use namespace.

### Primary class
First you need to create primary instance. It takes the arguments from command line and sets up the prefixes of commands.

> [!NOTE]
> Default prefixes are "--", "-" and "/", so `--input`, `-input` and `/input` are all the same command.

```csharp
var ar = new Argumentos.Args(args);
```

Without parameters it reads the arguments of the running program itself (`Environment.GetCommandLineArgs()` without the program path). `null` instead of the arguments is an empty list.
```csharp
var ar = new Argumentos.Args();
```

Own prefixes go in the second parameter (`null` keeps the default ones). The first of them is the one shown in the help screen.
```csharp
var ar = new Argumentos.Args(args, new string[] { "-", "--" });
```

> [!IMPORTANT]
> On Linux and macOS a path like `/tmp/x` starts with the default "/" prefix and would be taken as a command. If your program takes such paths, define own prefixes without "/".

### Commands
Command class represents recognized argument from command line (like -i or /input).

> [!NOTE]
> It is optional to use it, you can still read arguments without it (unless strict mode is on).

Create recognizable command. Name and description is mandatory, the name can be written with a prefix or without (`"input"` and `"--input"` are the same). You can make it required, required commands not found in argument list will throw exception.
```csharp
var cmd_input_file = new Argumentos.Command("input", "Input file path", true);
ar.AddCommand(cmd_input_file);
```

Adding a command with a name that is already there throws an exception. To change a command on purpose, remove or replace it.
```csharp
ar.RemoveCommand("debug");          // true when it was there
ar.ReplaceCommand(cmd_input_file);  // throws when there is no command with that name
```

You can add the values a command knows, each with its description. They show in the help screen and in strict mode no other value is accepted for that command. The same value twice throws an exception.

```csharp
var cmd_log_level = new Argumentos.Command("log", "Use to log program actions to a file");
cmd_log_level.AddArgumentValueDesc("normal", "(default) Enables logging at normal level");
cmd_log_level.AddArgumentValueDesc("warning", "Log only warning and up events");
cmd_log_level.AddArgumentValueDesc("error", "Log only errors");
ar.AddCommand(cmd_log_level);
```

A command that never takes a value is a flag. Words after a flag do not belong to it, they are positional arguments.
```csharp
var cmd_verbose = new Argumentos.Command("verbose", "Prints more details");
cmd_verbose.SetFlag();
ar.AddCommand(cmd_verbose);
```

### Strict mode
Off by default. When on, `Process()` throws an exception for any command that was not added with `AddCommand`, for any value outside the documented values of its command (commands without documented values take anything) and for a command given more than once. Positional arguments are allowed. What to do with the exception is up to you.

```csharp
ar.SetStrictMode();
```

### Case-insensitive names
Off by default. When on, `/Help`, `-HELP` and `help` are the same command, and documented values match regardless of case too.

```csharp
ar.SetIgnoreCase();
```

### Processing
Call processing function to pair commands with values and to check required commands.

```csharp
ar.Process();
```
Has to be called before reading anything. How the words on the command line are split:
- every word after a command up to the next prefixed one is its value — `-files a.txt b.txt -debug` gives `files` two values and `debug` none
- a value can also follow a separator — `--log=warning`, `/log:warning`
- words that belong to no command (before the first command, after a flag) are positional arguments
- everything after `--` is positional, even when it starts with a prefix
- a prefixed number (`-count -5`) and a bare prefix (`-`) are words, not commands
- a command given more than once collects all its values (`-i a -i b`), unless strict mode is on

### Getting values from command line arguments
You can access value directly or check if it is defined.

> [!NOTE]
> You can use key with prefixes or not.

Check if command is present.
```csharp
if (ar.HasArgument("--debug"))
    Console.WriteLine("Debug has been set!");
```

Or get its value.
```csharp
var value = ar.GetArgument<string>("log");
```

> [!NOTE]
> Supported types are: string, bool, int, long, float and double. Any other type throws an exception.

```csharp
int count = ar.GetArgument<int>("count");
bool debug = ar.GetArgument<bool>("debug");
```
A missing command returns the default of the type (`null`, `0`, `false`), a value that cannot be read as the type throws an exception. A command without a value reads as `true`, decimals use a dot (`1.5`). With several values `GetArgument` returns the first one.

All values of a command as a list (empty for a missing command or one without values).
```csharp
List<string> files = ar.GetValues("files");
```

Positional arguments in their order.
```csharp
List<string> inputs = ar.GetPositional();
```

### Help screen
Get the help text, or write it straight to the console.

```csharp
string help = ar.GetHelp();
ar.PrintHelp();
```

Every registered command shows there with the first prefix, each documented value under its command:
```
  --input - Input file path
  --log (optional) - Use to log program actions to a file
      Recognized values:
      - warning -> Log only warning and up events
```
When and on which command to show it (`--help`, `-?`, after an error) is up to you.

### Errors
Everything the library refuses is an `ArgsException` — required command missing, strict mode, a value that cannot be read, a duplicate command, reading before `Process()`. Its message is meant to be shown to the user as it is.
