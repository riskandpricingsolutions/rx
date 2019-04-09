using NUnit.Framework;
using System;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using static System.Console;

namespace RiskAndPricingSolutions.Rx.Expositional.OverviewOfRx.Operators.Combining
{
    [TestFixture]
    public class AmbTest
    {
        [Test]
        public void TestAmbTest()
        {
            Subject<string> a = new Subject<string>();
            Subject<string> b = new Subject<string>();

            Observable.Amb(a, b)
                .Subscribe(WriteLine);

            a.OnNext("a");
            b.OnNext("1");
            a.OnNext("b");
            b.OnNext("2");
        }
    }
}