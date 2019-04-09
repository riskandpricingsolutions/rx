using System;
using System.Reactive.Linq;
using NUnit.Framework;

namespace RiskAndPricingSolutions.Rx.Expositional.OverviewOfRx.Operators.Combining
{
    [TestFixture]
    public class RepeatTest
    {
        [Test]
        public void TestRepeatTest()
        {
            Observable
                .Range(0, 2)
                .Repeat(2)
                .Subscribe(Console.WriteLine, () => Console.WriteLine("OnCompleted\n"));
        }
    }
}