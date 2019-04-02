using System;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using NUnit.Framework;

namespace RiskAndPricingSolutions.Rx.Expositional.OverviewOfRx.Basics.HotAndCold
{
    [TestFixture]
    public class ConnectableObservableTest
    {
        [Test]
        public void TestConnectableObservable()
        {
            // As per the previous sample we use a factory method together
            // with Observable.Create to create an IObservable which delivers
            // completely different values from each subscription. The key 
            // difference is that we wrap this Observable 
            // with a ConnectableObservable 
            // using the Publish extension method. This extra layer allows us to
            // share the values published from the originating Observable as the
            // Connectable wrapper performs the multiplexing
            int x = 0;

            // Define a factory method that when invoked directly calls OnNext
            IDisposable FactMethod(IObserver<int> observer)
            {
                observer.OnNext(x++);
                return Disposable.Empty;
            }

            // Use the Observable.Create to turn our factory method into an IObservable
            IObservable<int> observable = Observable.Create((Func<IObserver<int>, IDisposable>)FactMethod);

            // Wrap the source Observable in a Connectable observable
            IConnectableObservable<int> connectableObservable = observable.Publish();

            // Even though we subscribe twice the connectable observable 
            // will make sure
            // there is only one underlying Observable 
            // with the ConnectableObservable
            // providing multi-plexing
            connectableObservable.Subscribe(i => Console.WriteLine($"A {i}"));
            connectableObservable.Subscribe(i => Console.WriteLine($"B {i}"));

            // The subscription is now carried out and multiplexed out to the
            // registered observers
            connectableObservable.Connect();
        }
    }
}