using System;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading;
using NUnit.Framework;
using RiskAndPricingSolutions.Rx.SharedResources.BasicImplementations;

namespace RiskAndPricingSolutions.Articles.Rx.Basics
{
    [TestFixture]
    public class HotAndColdObservables
    {
        [Test]
        public void BasicColdObservable()
        {
            // We use a factory method together with Observable.Create to create
            // an IObservable which delivers completely different values to each 
            // Subscription. This is the basic property of a ColdObservable. Nothing is 
            // delivered util a subscription is made and each subscription gets a different
            // value.
            int x = 0;

            // Define a factory method that when invoked directly calls OnNext
            IDisposable FactMeth(IObserver<int> observer)
            {
                observer.OnNext(x++);
                return Disposable.Empty;
            }

            // Use Observable.Create to turn our factory method into an IObservable
            var observable = Observable.Create((Func<IObserver<int>, IDisposable>)FactMeth);

            // Perform two different subscriptions. Each IOBserver get different values
            // due to the nature of a cold observable
            observable.Subscribe(i => Console.WriteLine($"A {i}"));
            observable.Subscribe(i => Console.WriteLine($"B {i}"));
        }

        [Test]
        public void BasicConnectableObservable()
        {
            // As per the previous sample we use a factory method together
            // with Observable.Create to create an IObservable which delivers
            // completely different values from each subscription. The key 
            // difference is that we wrap this Observable with a ConnectableObservable 
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

            // Even though we subscribe twice the connectable observable will make sure
            // there is only one underlying Observable with the ConnectableObservable
            // providing multi-plexing
            connectableObservable.Subscribe(i => Console.WriteLine($"A {i}"));
            connectableObservable.Subscribe(i => Console.WriteLine($"B {i}"));

            // The subscription is now carried out and multiplexed out to the
            // registered observers
            connectableObservable.Connect();
        }


        [Test]
        public void HotObservable()
        {
            // In this example we wrap our observable in a ConnectableObservable 
            // and connect it before we make any subscriptions. By doing this we are
            // creating what is known as a Hot Observable. This Observable is still
            // publishing values even though it has no subscriptions.
            SimpleObservable<int> sourceObservable = new SimpleObservable<int>();

            IConnectableObservable<int> hotObservable = sourceObservable
                .Do(i => Console.WriteLine("Source({0}) thread {1}", i, Thread.CurrentThread.ManagedThreadId))
                .Publish();

            // Connecting to the IConnectableObservable causes it to subscribe on
            // the sourceObservable thereby setting up a hot observable which will
            // publish out even when the connectableObservable has no observers
            IDisposable disposable = hotObservable.Connect();

            // Notice this is logged via the Do call even though we have no observer on
            // the connectableObservable
            sourceObservable.Publish(1);

            // now we subscribe on the connectableObservable
            hotObservable.Subscribe(i => Console.WriteLine("OnNext({0}) thread {1}", i, Thread.CurrentThread.ManagedThreadId));

            // this is now delivered to the IObserver
            sourceObservable.Publish(2);

            // Disposing of the connectableObservable turns off the publishing
            disposable.Dispose();
            sourceObservable.Publish(3);

            // we can now reconnect to the same IConnectableObservable and once again
            // messages are delivered
            disposable = hotObservable.Connect();
            sourceObservable.Publish(4);
        }
    }
}
