using Argumentos;

namespace Tests
{

    public static class CommandTests
    {

        public static void PrintHelpLeavesRequiredCommandUnmarked() {

            Command cmd = new Command("input", "Input file path", true);

            Check.Equal(Args.NewRow("- input - Input file path"), cmd.PrintHelp());
        }

        public static void PrintHelpMarksOptionalCommand() {

            Command cmd = new Command("input", "Input file path");

            Check.Equal(Args.NewRow("- (optional) input - Input file path"), cmd.PrintHelp());
        }

    }
}
