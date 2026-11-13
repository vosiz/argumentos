using System;
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

        public static void AddArgumentValueDescThrowsOnRepeatedValue() {

            Command cmd = new Command("log", "Logging level");
            cmd.AddArgumentValueDesc("warning", "Only warnings and up");

            Check.Throws<ArgsException>(() => cmd.AddArgumentValueDesc("warning", "Again"));
        }

        public static void ArgsExceptionKeepsMessageAndInner() {

            Exception inner = new Exception("inner");
            ArgsException exc = new ArgsException("outer", inner);

            Check.Equal("outer", exc.Message);
            Check.True(object.ReferenceEquals(inner, exc.InnerException));
        }

        public static void GetHelpLeavesRequiredCommandUnmarked() {

            Command cmd = new Command("input", "Input file path", true);

            Check.Equal("  input - Input file path" + Environment.NewLine, cmd.GetHelp());
        }

        public static void GetHelpMarksOptionalCommand() {

            Command cmd = new Command("input", "Input file path");

            Check.Equal("  --input (optional) - Input file path" + Environment.NewLine, cmd.GetHelp("--input"));
        }

        public static void GetHelpListsDocumentedValues() {

            Command cmd = new Command("log", "Logging level", true);
            cmd.AddArgumentValueDesc("warning", "Only warnings and up");

            string expected =
                "  log - Logging level" + Environment.NewLine +
                "      Recognized values:" + Environment.NewLine +
                "      - warning -> Only warnings and up" + Environment.NewLine;

            Check.Equal(expected, cmd.GetHelp());
        }

    }
}
