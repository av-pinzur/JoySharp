using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AvP.Joy.Test
{
    public class HelpedTest
    {
        protected void AssertThrows(Type expectedExceptionType, Action action)
        {
            AssertThrows(expectedExceptionType, action, null, null);
        }

        protected void AssertThrows(Type expectedExceptionType, Action action, string message)
        {
            AssertThrows(expectedExceptionType, action, message, null);
        }

        protected void AssertThrows(Type expectedExceptionType, Action action, string message, params object[] formatArgs)
        {
            AssertThrows(expectedExceptionType, AsVoidish(action), message, formatArgs);
        }

        protected void AssertThrows(Type expectedExceptionType, string expectedExceptionMessage, Action action, string message, params object[] formatArgs)
        {
            AssertThrows(expectedExceptionType, expectedExceptionMessage, AsVoidish(action), message, formatArgs);
        }

        protected void AssertThrows<TResult>(Type expectedExceptionType, Func<TResult> function)
        {
            AssertThrows(expectedExceptionType, function, null, null);
        }

        protected void AssertThrows<TResult>(Type expectedExceptionType, Func<TResult> function, string message)
        {
            AssertThrows(expectedExceptionType, function, message, null);
        }

        protected void AssertThrows<TResult>(Type expectedExceptionType, Func<TResult> function, string message, params object[] formatArgs)
        {
            AssertThrows(expectedExceptionType, null, function, message, formatArgs);
        }

        protected void AssertThrows<TResult>(Type expectedExceptionType, string expectedExceptionMessage, Func<TResult> function, string message = null, params object[] formatArgs)
        {
            try
            {
                TResult result = function();
                Assert.Fail(BuildCompositeMessage(message, formatArgs, result is Voidish
                        ? $"Expected {expectedExceptionType} not thrown."
                        : $"Expected {expectedExceptionType} not thrown; returned instead: [{result}]"));
            }
            catch (Exception e) when (expectedExceptionType.IsAssignableFrom(e.GetType()))
            {
                if (expectedExceptionMessage != null)
                    Assert.AreEqual(expectedExceptionMessage, e.Message, BuildCompositeMessage(message, formatArgs, "Unexpected exception message."));
            }
        }

        protected void AssertEqualThrows(Action expected, Action actual, string message = null, params object[] formatArgs)
        {
            AssertEqualThrows(AsVoidish(expected), AsVoidish(actual), message, formatArgs);
        }

        protected void AssertEqualThrows<TResult>(Func<TResult> expected, Func<TResult> actual, string message = null, params object[] formatArgs)
        {
            var expectedResult = CatchingAll(expected);
            if (expectedResult.Item2 == null)
                throw new ArgumentException(
                    expectedResult.Item1 is Voidish
                        ? "Provided delegate must throw an exception."
                        : $"Provided delegate must throw an exception; instead returned [{expectedResult.Item1}].", 
                    "expected");

            AssertThrows(expectedResult.Item2.GetType(), expectedResult.Item2.Message, actual, message, formatArgs);
        }

        private Tuple<TResult, Exception> CatchingAll<TResult>(Func<TResult> function)
        {
            try
            {
                return new Tuple<TResult, Exception>(function(), null);
            }
            catch (Exception e)
            {
                return new Tuple<TResult, Exception>(default(TResult), e);
            }
        }

        private static string BuildCompositeMessage(string userMessage, object[] userFormatArgs, string detailMessage, params object[] detailFormatArgs)
        {
            userMessage = userMessage.TrimToNull();
            userFormatArgs = userFormatArgs ?? new object[0];

            var userPrefix = userMessage != null 
                ? userFormatArgs.Any()
                    ? ' ' + string.Format(userMessage, userFormatArgs) 
                    : ' ' + userMessage 
                : string.Empty;

            return userPrefix + ' ' + (detailFormatArgs.Any()
                ? string.Format(detailMessage, detailFormatArgs)
                : detailMessage );
        }

        private struct Voidish { }

        private Func<Voidish> AsVoidish(Action action)
        {
            return () =>
            {
                action();
                return default(Voidish);
            };
        }
    }
}