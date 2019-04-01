using System;
using System.Reactive.Linq;
using NUnit.Framework;
using static System.Console;

namespace RiskAndPricingSolutions.Rx.Expositional.OverviewOfRx.Operators.Inspecting
{
    [TestFixture]
    public class DefaultIfEmpty
    {
        [Test]
        public void TestDefaultIfEmpty()
        {
            IObservable<int> result = Observable.Empty<int>().DefaultIfEmpty(-1);
            result.Subscribe(WriteLine);
        }
    }
}