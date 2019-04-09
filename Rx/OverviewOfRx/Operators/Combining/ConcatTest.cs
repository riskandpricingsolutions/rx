using System.Reactive.Linq;
using NUnit.Framework;
using static System.Console;
using System;

namespace RiskAndPricingSolutions.Rx.Expositional.OverviewOfRx.Operators.Combining
{
    [TestFixture]
    public class ConcatTest
    {
        [Test]
        public void Concat()
        {
            Observable
                .Range(1, 2)
                .Concat(Observable.Range(5, 2))
                .Subscribe(WriteLine);
        }
    }
}