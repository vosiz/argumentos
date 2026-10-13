using System.Reflection;
using ConsoleZapp;

namespace Tests
{
    public class Program
    {
        // Entry point
        private static void Main(string[] args)
        {

            Cli.Init();

            TestRunner.RunAll(Assembly.GetExecutingAssembly());

            System.Console.ReadLine();
        }
    }
}
