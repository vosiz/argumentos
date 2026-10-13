using Argumentos;

namespace Tests
{

    public static class ArgsTests
    {

        public static void GetArgumentAcceptsPrefixedKey() {

            Args ar = CreateProcessed("-input", "somefile.txt");

            Check.Equal("somefile.txt", ar.GetArgument<string>("-input"));
        }

        public static void GetArgumentReadsBoolFromArgumentWithoutValue() {

            Args ar = CreateProcessed("-debug");

            Check.True(ar.GetArgument<bool>("debug"));
        }

        public static void GetArgumentReadsFloat() {

            Args ar = CreateProcessed("-ratio", "1.5");

            Check.Equal(1.5f, ar.GetArgument<float>("ratio"));
        }

        public static void GetArgumentReadsInt() {

            Args ar = CreateProcessed("-count", "42");

            Check.Equal(42, ar.GetArgument<int>("count"));
        }

        public static void GetArgumentReturnsDefaultForUnknownValueTypeKey() {

            Args ar = CreateProcessed("-input", "somefile.txt");

            Check.Equal(0, ar.GetArgument<int>("count"));
            Check.False(ar.GetArgument<bool>("debug"));
        }

        public static void GetArgumentReturnsNullForUnknownKey() {

            Args ar = CreateProcessed("-input", "somefile.txt");

            Check.Equal(null, ar.GetArgument<string>("output"));
        }

        public static void GetArgumentThrowsBeforeProcess() {

            Args ar = new Args(new string[] { "-input", "somefile.txt" });

            Check.Throws<ArgsException>(() => ar.GetArgument<string>("input"));
        }

        public static void GetArgumentThrowsOnValueOfAnotherType() {

            Args ar = CreateProcessed("-count", "many");

            Check.Throws<ArgsException>(() => ar.GetArgument<int>("count"));
        }

        public static void HasArgumentAcceptsPrefixedKey() {

            Args ar = CreateProcessed("-input", "somefile.txt");

            Check.True(ar.HasArgument("input"));
            Check.True(ar.HasArgument("-input"));
        }

        public static void HasArgumentThrowsBeforeProcess() {

            Args ar = new Args(new string[] { "-input", "somefile.txt" });

            Check.Throws<ArgsException>(() => ar.HasArgument("input"));
        }

        public static void ProcessKeepsPrefixCharacterInsideName() {

            Args ar = new Args(new string[] { "--log-level", "warning" }, new string[] { "-", "--" });
            ar.AddCommand(new Command("log-level", "Logging level", true));
            ar.Process();

            Check.Equal("warning", ar.GetArgument<string>("log-level"));
        }

        public static void ProcessNamesRepeatedArgumentInError() {

            Args ar = new Args(new string[] { "-i", "a", "-i", "b" });
            string message = null;

            try {

                ar.Process();

            } catch (ArgsException exc) {

                message = exc.Message;
            }

            Check.Equal("ArgsExc: Argument \"i\" is defined more than once", message);
        }

        public static void ProcessPairsArgumentWithValue() {

            Args ar = CreateProcessed("-input", "somefile.txt", "debug");

            Check.Equal("somefile.txt", ar.GetArgument<string>("input"));
        }

        public static void ProcessStoresArgumentWithoutValueAsOne() {

            Args ar = CreateProcessed("-input", "somefile.txt", "debug");

            Check.True(ar.HasArgument("debug"));
            Check.Equal("1", ar.GetArgument<string>("debug"));
        }

        public static void ProcessTakesNegativeNumberAsValue() {

            Args ar = CreateProcessed("-count", "-5");

            Check.Equal(-5, ar.GetArgument<int>("count"));
            Check.False(ar.HasArgument("5"));
        }

        public static void ProcessThrowsWhenRequiredCommandIsMissing() {

            Args ar = new Args(new string[] { "-debug" });
            ar.AddCommand(new Command("input", "Input file path", true));

            Check.Throws<ArgsException>(ar.Process);
        }

        private static Args CreateProcessed(params string[] args) {

            Args ar = new Args(args);
            ar.Process();

            return ar;
        }

    }
}
