# Argumentos
A command line arguments processing tool.

Auto-processes arguments, simple interface to setup.

## Usage
Use namespace **Argumentos**. Implicetly or by including via "using";

Steps are:
1) Creating primary instance
2) (Optional) Create, describe and add command
3) Process
4) Get argument value or check existance

More examples in ArgsTesting project.

### Simplistic example
```csharp
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
