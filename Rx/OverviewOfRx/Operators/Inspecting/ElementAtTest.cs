using System;
using System.Reactive.Linq;
using NUnit.Framework;
using static System.Console;

namespace RiskAndPricingSolutions.Rx.Expositional.OverviewOfRx.Operators.Inspecting
{
    [TestFixture]
    public class ElementAt
    {
        [Test]
        public void TestElementAt()
        {
            IObservable<int> elementAt = Observable
                .Range(0,3)
                .ElementAt(2);

            elementAt.Subscribe(WriteLine);
        }
    }
}