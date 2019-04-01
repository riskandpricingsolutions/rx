using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using NUnit.Framework;
using static System.Console;


namespace RiskAndPricingSolutions.Rx.Expositional.OverviewOfRx.Operators.Inspecting
{
    [TestFixture]
    public class ContainsTest
    {
        [Test]
        public void TestContainsTest()
        {
            IObservable<bool> result = Observable.Range(0, 3)
                .Contains(1);

            result.Subscribe(WriteLine);

            IObservable<bool> result2 = Observable.Range(0, 3)
                .Contains(4);
            result2.Subscribe(WriteLine);

            IObservable<bool> result3 = new[] {"hello", "world"}
                .ToObservable()
                .Contains("HELLO", StringComparer.InvariantCultureIgnoreCase);
            result3.Subscribe(WriteLine);
        }
    }
}