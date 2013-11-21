using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Runtime.Serialization;
using System.Security;

namespace Xunit.Sdk
{
    /// <summary>
    /// Base class for exceptions that have actual and expected values
    /// </summary>
    [SuppressMessage("Microsoft.Design", "CA1032:ImplementStandardExceptionConstructors")]
    [Serializable]
    public class AssertActualExpectedException : XunitException
    {
        /// <summary>
        /// Creates a new instance of the <see href="AssertActualExpectedException"/> class.
        /// </summary>
        /// <param name="expected">The expected value</param>
        /// <param name="actual">The actual value</param>
        /// <param name="userMessage">The user message to be shown</param>
        public AssertActualExpectedException(object expected, object actual, string userMessage)
            : base(userMessage)
        {
            Actual = actual == null ? null : ConvertToString(actual);
            Expected = expected == null ? null : ConvertToString(expected);

            if (actual != null &&
                expected != null &&
                Actual == Expected &&
                actual.GetType() != expected.GetType())
            {
                Actual += String.Format(CultureInfo.CurrentCulture, " ({0})", actual.GetType().FullName);
                Expected += String.Format(CultureInfo.CurrentCulture, " ({0})", expected.GetType().FullName);
            }
        }

        /// <inheritdoc/>
        protected AssertActualExpectedException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            Actual = info.GetString("Actual");
            Expected = info.GetString("Expected");
        }

        /// <summary>
        /// Gets the actual value.
        /// </summary>
        public string Actual { get; private set; }

        /// <summary>
        /// Gets the expected value.
        /// </summary>
        public string Expected { get; private set; }

        /// <summary>
        /// Gets a message that describes the current exception. Includes the expected and actual values.
        /// </summary>
        /// <returns>The error message that explains the reason for the exception, or an empty string("").</returns>
        /// <filterpriority>1</filterpriority>
        public override string Message
        {
            get
            {
                return String.Format(CultureInfo.CurrentCulture,
                                     "{0}{3}Expected: {1}{3}Actual:   {2}",
                                     base.Message,
                                     Expected ?? "(null)",
                                     Actual ?? "(null)",
                                     Environment.NewLine);
            }
        }

        static string ConvertToSimpleTypeName(Type type)
        {
            if (!type.IsGenericType)
                return type.Name;

            var simpleNames = type.GetGenericArguments().Select(ConvertToSimpleTypeName);
            var backTickIdx = type.Name.IndexOf('`');
            if (backTickIdx < 0)
                backTickIdx = type.Name.Length;  // F# doesn't use backticks for generic type names

            return type.Name.Substring(0, backTickIdx) + "<" + String.Join(", ", simpleNames) + ">";
        }

        static string ConvertToString(object value)
        {
            var stringValue = value as string;
            if (stringValue != null)
                return stringValue;

            var enumerableValue = value as IEnumerable;
            if (enumerableValue == null)
                return value.ToString();

            var valueStrings = new List<string>();

            foreach (object valueObject in enumerableValue)
            {
                string displayName;

                if (valueObject == null)
                    displayName = "(null)";
                else
                {
                    var stringValueObject = valueObject as string;
                    if (stringValueObject != null)
                        displayName = "\"" + stringValueObject + "\"";
                    else
                        displayName = valueObject.ToString();
                }

                valueStrings.Add(displayName);
            }

            return ConvertToSimpleTypeName(value.GetType()) + " { " + String.Join(", ", valueStrings.ToArray()) + " }";
        }

        /// <inheritdoc/>
        [SecurityCritical]
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            Assert.GuardArgumentNotNull("info", info);

            info.AddValue("Actual", Actual);
            info.AddValue("Expected", Expected);

            base.GetObjectData(info, context);
        }
    }
}