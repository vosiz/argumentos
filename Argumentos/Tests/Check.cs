using System;

namespace Tests
{

    public static class Check
    {

        // Fails unless the condition is true
        public static void True(bool condition, string message = "Expected true") {

            if (!condition)
                throw new Exception(message);
        }

        // Fails unless the condition is false
        public static void False(bool condition, string message = "Expected false") {

            if (condition)
                throw new Exception(message);
        }

        // Fails unless expected and actual are equal
        public static void Equal<T>(T expected, T actual, string message = null) {

            if (!object.Equals(expected, actual))
                throw new Exception(message ?? string.Format("Expected '{0}', got '{1}'", expected, actual));
        }

        // Fails unless the action throws TException
        public static void Throws<TException>(Action action, string message = null) where TException : Exception {

            try {

                action();

            } catch (TException) {

                return;
            }

            throw new Exception(message ?? string.Format("Expected {0} to be thrown", typeof(TException).Name));
        }

    }
}
