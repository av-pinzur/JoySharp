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
            try
            {
                action();
                Assert.Fail(BuildCompositeMessage(message, formatArgs, "The expected exception was not thrown."));
            }
            catch (Exception e)
            {
                Assert.IsInstanceOfType(e, expectedExceptionType, BuildCompositeMessage(message, formatArgs, "Incorrect exception type was thrown: {0}", e));
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

            return userPrefix + (detailFormatArgs.Any()
                ? string.Format(detailMessage, detailFormatArgs)
                : detailMessage );
        }
    }
}