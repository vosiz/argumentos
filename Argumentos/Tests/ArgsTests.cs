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

        public static void GetPositionalThrowsBeforeProcess() {

            Args ar = new Args(new string[] { "input.txt" });

            Check.Throws<ArgsException>(() => ar.GetPositional());
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
            ar.AddCommand(new Command("i", "Include path"));
            ar.SetStrictMode();
            string message = null;

            try {

                ar.Process();

            } catch (ArgsException exc) {

                message = exc.Message;
            }

            Check.Equal("ArgsExc: Argument \"i\" is defined more than once", message);
        }

        public static void ProcessPairsArgumentWithValue() {

            Args ar = CreateProcessed("-input", "somefile.txt", "-debug");

            Check.Equal("somefile.txt", ar.GetArgument<string>("input"));
        }

        public static void ProcessRecognizesAllDefaultPrefixes() {

            Args ar = CreateProcessed("--alpha", "1", "-beta", "2", "/gamma", "3");

            Check.Equal("1", ar.GetArgument<string>("alpha"));
            Check.Equal("2", ar.GetArgument<string>("beta"));
            Check.Equal("3", ar.GetArgument<string>("gamma"));
        }

        public static void ProcessStoresArgumentWithoutValueAsOne() {

            Args ar = CreateProcessed("-input", "somefile.txt", "-debug");

            Check.True(ar.HasArgument("debug"));
            Check.Equal("1", ar.GetArgument<string>("debug"));
        }

        public static void ProcessTakesNegativeNumberAsValue() {

            Args ar = CreateProcessed("-count", "-5");

            Check.Equal(-5, ar.GetArgument<int>("count"));
            Check.False(ar.HasArgument("5"));
        }

        public static void ProcessTakesValuesUpToNextPrefix() {

            Args ar = CreateProcessed("-files", "a.txt", "b.txt", "c.txt", "-debug");

            Check.Equal("a.txt|b.txt|c.txt", string.Join("|", ar.GetValues("files")));
            Check.Equal("a.txt", ar.GetArgument<string>("files"));
            Check.True(ar.HasArgument("debug"));
        }

        public static void ProcessAddsUpRepeatedArgumentWithoutStrictMode() {

            Args ar = CreateProcessed("-i", "a", "-i", "b");

            Check.Equal("a|b", string.Join("|", ar.GetValues("i")));
        }

        public static void ProcessCollectsWordsBeforeFirstCommandAsPositional() {

            Args ar = CreateProcessed("input.txt", "output.txt", "-v");

            Check.Equal("input.txt|output.txt", string.Join("|", ar.GetPositional()));
            Check.False(ar.HasArgument("input.txt"));
            Check.True(ar.HasArgument("v"));
        }

        public static void ProcessSendsWordsAfterFlagToPositional() {

            Command cmd_verbose = new Command("verbose", "Verbose output");
            cmd_verbose.SetFlag();

            Args ar = new Args(new string[] { "-verbose", "input.txt" });
            ar.AddCommand(cmd_verbose);
            ar.Process();

            Check.True(ar.GetArgument<bool>("verbose"));
            Check.Equal(0, ar.GetValues("verbose").Count);
            Check.Equal("input.txt", string.Join("|", ar.GetPositional()));
        }

        public static void ProcessTakesEverythingAfterDoubleDashAsPositional() {

            Args ar = CreateProcessed("-v", "--", "-not-a-command", "file.txt");

            Check.Equal("-not-a-command|file.txt", string.Join("|", ar.GetPositional()));
            Check.False(ar.HasArgument("not-a-command"));
        }

        public static void ProcessTakesBarePrefixAsWord() {

            Args ar = CreateProcessed("-", "-input", "-");

            Check.Equal("-", string.Join("|", ar.GetPositional()));
            Check.Equal("-", ar.GetArgument<string>("input"));
            Check.False(ar.HasArgument(""));
        }

        public static void ProcessReadsValueAfterSeparator() {

            Args ar = CreateProcessed("--log=warning", "/path:C:\\temp", "-count=-5");

            Check.Equal("warning", ar.GetArgument<string>("log"));
            Check.Equal("C:\\temp", ar.GetArgument<string>("path"));
            Check.Equal(-5, ar.GetArgument<int>("count"));
        }

        public static void ProcessIsCaseSensitiveByDefault() {

            Args ar = CreateProcessed("/Help");

            Check.False(ar.HasArgument("help"));
        }

        public static void IgnoreCaseMatchesCommandsAndValues() {

            Args ar = CreateStrict("/LOG", "Warning", "-Input", "a.txt");
            ar.SetIgnoreCase();
            ar.Process();

            Check.Equal("Warning", ar.GetArgument<string>("log"));
            Check.True(ar.HasArgument("input"));
        }

        public static void GetValuesReturnsEmptyForFlagAndUnknownKey() {

            Args ar = CreateProcessed("-debug");

            Check.Equal(0, ar.GetValues("debug").Count);
            Check.Equal(0, ar.GetValues("input").Count);
        }

        public static void StrictModeAcceptsDocumentedValue() {

            Args ar = CreateStrict("-log", "warning");

            ar.Process();

            Check.Equal("warning", ar.GetArgument<string>("log"));
        }

        public static void StrictModeAcceptsAnyValueWhenNoneDocumented() {

            Args ar = CreateStrict("-input", "a.txt", "b.txt");

            ar.Process();

            Check.Equal(2, ar.GetValues("input").Count);
        }

        public static void StrictModeThrowsOnUndocumentedValue() {

            Args ar = CreateStrict("-log", "verbose");

            Check.Throws<ArgsException>(ar.Process);
        }

        public static void StrictModeThrowsOnUnknownCommand() {

            Args ar = CreateStrict("-input", "a.txt", "-unknown");

            Check.Throws<ArgsException>(ar.Process);
        }

        public static void WithoutStrictModeUnknownCommandIsAccepted() {

            Args ar = new Args(new string[] { "-unknown", "x" });
            ar.AddCommand(new Command("input", "Input file path"));
            ar.Process();

            Check.Equal("x", ar.GetArgument<string>("unknown"));
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

        private static Args CreateStrict(params string[] args) {

            Command cmd_log = new Command("log", "Logging level");
            cmd_log.AddArgumentValueDesc("warning", "Only warnings and up");
            cmd_log.AddArgumentValueDesc("error", "Only errors");

            Args ar = new Args(args);
            ar.AddCommand(cmd_log);
            ar.AddCommand(new Command("input", "Input file path"));
            ar.SetStrictMode();

            return ar;
        }

    }
}
