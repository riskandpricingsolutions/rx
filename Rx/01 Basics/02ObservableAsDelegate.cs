using System;
using NUnit.Framework;
using RiskAndPricingSolutions.Rx.SharedResources.BasicImplementations;

namespace RiskAndPricingSolutions.Articles.Rx.Basics
{
    [TestFixture]
    public class ObservableAsDelegate
    {
        [Test]
        public void WeCanUseADelegateAsAnObservable()
        {
            var observable = new SimpleObservable<int>();
            observable.Subscribe(i => Console.WriteLine($"Delegate OnNext({i})"));
            observable.Publish(1);
            observable.Publish(2);
        }
    }
}