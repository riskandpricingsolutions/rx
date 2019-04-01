using System;
using System.Reactive.Linq;
using NUnit.Framework;
using static System.Console;

namespace RiskAndPricingSolutions.Rx.Expositional.OverviewOfRx.Operators.Inspecting
{
    [TestFixture]
    public class AllTest
    {
        [Test]
        public void AllFailesIfAnyElementsFailPredicate()
        {
            IObservable<bool> result = Observable
                .Range(0, 3)
                .All(x => x < 2);

            result.Subscribe(WriteLine);
        }

        [Test]
        public void AllOnEmptySequenceReturnsTrue()
        {
            IObservable<bool> result = Observable
                .Empty<int>()
                .All(x => x < 3);

            result.Subscribe(WriteLine);
        }
    }
}