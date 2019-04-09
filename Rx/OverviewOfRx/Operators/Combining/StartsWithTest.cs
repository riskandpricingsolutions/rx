using NUnit.Framework;
using System;
using System.Reactive.Linq;
using static System.Console;

namespace RiskAndPricingSolutions.Rx.Expositional.OverviewOfRx.Operators.Combining
{
    [TestFixture]
    public class StartsWith
    {
        [Test]
        public void TestStartsWith()
        {
            Observable
                .Range(10, 2)
                .StartWith(8, 9)
                .Subscribe(WriteLine);
        }
    }
}