using System;
using System.Linq;
using System.Reflection;

namespace Tests
{

    public static class TestRunner
    {

        // Runs every test method found in the assembly
        // - public static parameterless void, on any *Tests class
        public static void RunAll(Assembly assembly) {

            var test_classes = assembly.GetTypes()
                .Where(t => t.IsClass && t.IsAbstract && t.IsSealed && t.Name.EndsWith("Tests"))
                .OrderBy(t => t.FullName);

            var tests = test_classes
                .SelectMany(test_class => test_class.GetMethods(BindingFlags.Public | BindingFlags.Static)
                    .Where(m => m.GetParameters().Length == 0 && m.ReturnType == typeof(void))
                    .OrderBy(m => m.Name)
                    .Select(method => new { Class = test_class, Method = method }))
                .ToList();

            int cap = tests.Count;
            int index = 0;
            int passed = 0;
            int failed = 0;

            foreach (var test in tests) {

                index++;

                string name = string.Format("{0}.{1}", test.Class.FullName, test.Method.Name);
                string counter = string.Format("{0,4}/{1,4}", index, cap);

                try {

                    test.Method.Invoke(null, null);
                    Console.WriteLine(string.Format("{0} [PASS] {1}", counter, name));
                    passed++;

                } catch (TargetInvocationException exc) {

                    Console.WriteLine(string.Format("{0} [FAIL] {1}", counter, name));
                    WriteFailure(exc.InnerException ?? exc);
                    failed++;
                }
            }

            Console.WriteLine();
            Console.WriteLine(string.Format("Passed: {0}, Failed: {1}", passed, failed));
        }

        // Prints a failed test's exception in red
        private static void WriteFailure(Exception exc) {

            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(exc.Message);

            if (exc.InnerException != null)
                Console.WriteLine(string.Format("- {0}", exc.InnerException.Message));

            Console.ResetColor();
        }

    }
}
