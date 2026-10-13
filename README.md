# Argumentos
Small library to parse and process arguments from command-line

## Motivation
Every console or form app ends up reading its command-line arguments by hand sooner or later. Argumentos does the boring part once — pairs arguments with their values, checks the required ones and puts together a help screen — so it does not have to be written again in each C#/.Net solution.

MIT — use it however you like.

## Requirements
- .NET Framework 4.8

## Bug tracker
No tracked

## Features
- Pairing of arguments and values, with own argument prefixes
- Commands — recognized arguments with description, required ones and documented values
- Typed reads of values and a generated help screen

See [Roadmap](#roadmap) for full details.

## Roadmap
- [x] Pairing of arguments and values
- [x] Own argument prefixes
- [x] Commands (recognized arguments with description)
- [x] Required arguments
- [x] Documented argument values
- [x] Typed reads (int, float, bool, string)
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
First you need to create primary instance.
It will take arguments from command line and setup default prefixes to argument IDs.

> [!NOTE]
> Default prefixes are "-" and "/"

```csharp
var ar = new Argumentos.Args(args);
```

> [!TIP]
> Define own prefixes (like --help) via optional parameter in primary class instance making.

```csharp
var ar = new Argumentos.Args(args, new string[] { "-", "--"});
```

### Commands
Command class represents recognized argument from command line (like -i or /input).

> [!NOTE]
> It is optional to use it, you can still recognize arguments without it.

Create recognizable command. Name and description is mandatory. You can make it required, required fields not found in argument list will throw exception.
```csharp
var cmd_input_file = new Argumentos.Command("input", "Input file path", true);
ar.AddCommand(cmd_input_file);
```

You can add desired values of argument with their descriptions.

```csharp
var cmd_log_level = new Argumentos.Command("log", "Use to log program actions to a file");
cmd_log_level.AddArgumentValueDesc("warning", "Log only warning and up events");
cmd_log_level.AddArgumentValueDesc("error", "Log only errors");
cmd_log_level.AddArgumentValueDesc("(default) normal", "Enables logging at normal level");
ar.AddCommand(cmd_log_level);
```

### Processing
Call processing function to pair arguments with values and to check required fields.

```csharp
ar.Process();
```
Has to be called before `HasArgument`/`GetArgument`. An argument given more than once throws an exception, a prefixed number (`-count -5`) is taken as a value.

> [!TIP]
> Values starting with "/" (like paths) collide with the default prefix - define own prefixes without it.

### Getting values from command line arguments
You can access value directly or check if it is defined.

> [!NOTE]
> You can use key with prefixes or not.

Check if value is set - key argument is present.
```csharp
if (ar.HasArgument("--debug"))
    Console.WriteLine("Debug has been set!");
```

Or get value of desired value.
```csharp
var value = ar.GetArgument<string>("log");
```

> [!NOTE]
> Supported values are: int, float, bool and string.

```csharp
int count = ar.GetArgument<int>("count");
bool debug = ar.GetArgument<bool>("debug");
```
A missing argument returns the default of the type (`null`, `0`, `false`), a value of another type throws an exception. An argument without a value reads as `true`, decimals use a dot (`1.5`).

### Print HELP screen
An option to show commands-help screen.

```csharp
string help = ar.PrintHelp();
Console.WriteLine(help);
```

Every registered command will show there. Each value defined for each argument will show there too.
