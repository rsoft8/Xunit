using System;
using System.ComponentModel;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace TestUtility
{
    public class CulturedXunitTestCase : XunitTestCase
    {
        private string culture;

        [EditorBrowsable(EditorBrowsableState.Never)]
        [Obsolete("Called by the de-serializer", error: true)]
        public CulturedXunitTestCase() { }

        public CulturedXunitTestCase(TestMethodDisplay defaultMethodDisplay, ITestMethod testMethod, string culture)
            : base(defaultMethodDisplay, testMethod)
        {
            Initialize(culture);
        }

        void Initialize(string culture)
        {
            this.culture = culture;

            Traits.Add("Culture", culture);

            DisplayName += String.Format(" [{0}]", culture);
        }

        public override void Deserialize(IXunitSerializationInfo data)
        {
            base.Deserialize(data);

            Initialize(data.GetValue<string>("Culture"));
        }

        public override void Serialize(IXunitSerializationInfo data)
        {
            base.Serialize(data);

            data.AddValue("Culture", culture);
        }

        public override async Task<RunSummary> RunAsync(IMessageBus messageBus, object[] constructorArguments, ExceptionAggregator aggregator, CancellationTokenSource cancellationTokenSource)
        {
            var originalCulture = Thread.CurrentThread.CurrentCulture;
            var originalUICulture = Thread.CurrentThread.CurrentUICulture;

            try
            {
                var cultureInfo = CultureInfo.GetCultureInfo(culture);
                Thread.CurrentThread.CurrentCulture = cultureInfo;
                Thread.CurrentThread.CurrentUICulture = cultureInfo;

                return await base.RunAsync(messageBus, constructorArguments, aggregator, cancellationTokenSource);
            }
            finally
            {
                Thread.CurrentThread.CurrentCulture = originalCulture;
                Thread.CurrentThread.CurrentUICulture = originalUICulture;
            }
        }
    }
}
