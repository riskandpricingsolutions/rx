using System;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using NUnit.Framework;

namespace RiskAndPricingSolutions.Rx.Expositional.OverviewOfRx.Basics.HotAndCold
{
    [TestFixture]
    public class ColdObservable
    {
        [Test]
        public void TestColdObservable()
        {
            // We use a factory method together with Observable.Create to create
            // an IObservable which delivers completely different values to each 
            // Subscription. This is the basic property of a ColdObservable. 
            // Nothing is delivered util a subscription is made and each 
            // subscription gets a different value.
            int x = 0;

            // Define a factory method that when invoked directly calls OnNext
            IDisposable FactMeth(IObserver<int> observer)
            {
                observer.OnNext(x++);
                return Disposable.Empty;
            }

            // Use Observable.Create to turn our factory method into an IObservable
            var observable = Observable.Create((Func<IObserver<int>, IDisposable>)FactMeth);

            // Perform two different subscriptions. Each IOBserver 
            // get different values to the nature of a cold observable
            observable.Subscribe(i => Console.WriteLine($"A {i}"));
            observable.Subscribe(i => Console.WriteLine($"B {i}"));
        }
    }
}