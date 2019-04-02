using System;
using System.Reactive.Subjects;
using System.Threading;
using NUnit.Framework;
using RiskAndPricingSolutions.Rx.SharedResources.BasicImplementations;
using System.Reactive.Linq;

namespace RiskAndPricingSolutions.Rx.Expositional.OverviewOfRx.Basics.HotAndCold
{
    [TestFixture]
    public class HotObservable
    {
        [Test]
        public void TestHotObservable()
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

            // Tis logged via the Do call even though we have no observer on
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