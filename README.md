# Argumentos
Small library to parse and process arguments from command-line

For console and form-apps of C#/.Net solutions.

At least **.Net 4.8** is neeaded.

## Install
### Nuget
In visual studio nuget package manager search for "Vosiz" or "Argumentos".

## Usage
Include **Argumentos** by using or use namespace.

Library processes and provides arguments and its values.

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

Create recognizable command. Name and description is mandatory. You can make it required, required fields nto found in argument list will throw exception.
```csharp
var cmd_input_file = new Argumentos.Command("input", "Input file path", true);
ar.AddCommand(cmd_input_file);
```

You can add desired values of argument with they descriptions.

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

### Print HELP screen
An option to show commnads-help screen.

```csharp
ar.PrintHelp();
```

Every registered command will show there. Each value defined for each argument will show there too.
