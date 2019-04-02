using System;
using System.Reactive.Concurrency;
using System.Threading;
using NUnit.Framework;
using RiskAndPricingSolutions.Rx.SharedResources.BasicImplementations;
using System.Reactive.Linq;

namespace RiskAndPricingSolutions.Rx.Expositional.OverviewOfRx.Basics.Scheduling
{
    [TestFixture]
    public class SchedulingExamples
    {
        [Test]
        public void DefaultScheduling()
        {
            // 1. Create the observable
            var observable = new SimpleObservable<int>();

            // 2. Create the observer
            IObserver<int> observer = new SimpleObserver<int>();

            // 3. Register the observer with the observable
            var disposable = observable.Subscribe(observer);

            // 4. Publish a value
            observable.Publish(1);

            // 5. Dispose the observer
            disposable.Dispose();

        }

        [Test]
        public void ExplicitSubscription()
        {
            // 1. Log out the calling thread
            "ExplicitMultiThreadedSubsciption".Log();
            Console.WriteLine("Current Thread {0}", Thread.CurrentThread.ManagedThreadId);

            // 2. Create a scheduler with its own thread 
            IScheduler scheduler = new SingleThreadedScheduler("KennyScheduler");

            // 3. Create the observable
            var observable = new SimpleObservable<int>();

            // 4. Create the observer
            IObserver<int> observer = new SimpleObserver<int>();

            // 5. Register the observer with the observable
            var disposable = observable.SubscribeOn(scheduler).Subscribe(observer);

            // Make sure the publish does not happen before the subscription as 
            // subscription is running on a separate thread
            Thread.Sleep(100);

            // 6. Publish a value
            observable.Publish(1);

            // 7. Dispose the observer
            disposable.Dispose();
        }

        [Test]
        public void ExplicitObservation()
        {
            // 1. Log out the calling thread
            "ExplicitMultiThreadedObservation".Log();

            // 2. Create a scheduler with its own thread and a 
            //    wait handle to prevent premature completion
            IScheduler scheduler = new SingleThreadedScheduler("KennysScheduler");
            AutoResetEvent handle = new AutoResetEvent(false);

            // 3. Create observable tell it we want to observer
            //    on our explicit scheduler
            var observable = new SimpleObservable<int>();
            var disposable = observable
                .ObserveOn(scheduler)
                .Subscribe(i => i.ToString().Log(), () => handle.Set());

            // 4. Publish 2 messages and then complete 
            observable.Publish(1);
            observable.Publish(2);
            observable.Complete();

            handle.WaitOne();
            disposable.Dispose();

        }
    }

}