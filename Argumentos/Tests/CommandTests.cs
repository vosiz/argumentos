using Argumentos;

namespace Tests
{

    public static class CommandTests
    {

        public static void AcceptsAnyValueWhenNoneDocumented() {

            Command cmd = new Command("input", "Input file path");

            Check.True(cmd.AcceptsValue("anything"));
        }

        public static void AcceptsOnlyDocumentedValues() {

            Command cmd = new Command("log", "Logging level");
            cmd.AddArgumentValueDesc("warning", "Only warnings and up");

            Check.True(cmd.AcceptsValue("warning"));
            Check.False(cmd.AcceptsValue("verbose"));
        }

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
