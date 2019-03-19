using System;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Threading;
using NUnit.Framework;
using RiskAndPricingSolutions.Rx.SharedResources.BasicImplementations;

namespace RiskAndPricingSolutions.Articles.Rx.Basics
{
    [TestFixture]
    public class Scheduling
    {
        [Test]
        public void DefaultScheduling()
        {
            // 1. Log out the calling thread
            nameof(DefaultScheduling).Log();

            // 3. Create the observable
            var observable = new SimpleObservable<int>();

            // 4. Create the observer
            IObserver<int> observer = new SimpleObserver<int>();

            // 5. Register the observer with the observable
            var disposable = observable.Subscribe(observer);

            // 6. Publish a value
            observable.Publish(1);

            // 7. Dispose the observer
            disposable.Dispose();
        }

        [Test]
        public void ExplicitMultiThreadedObservation()
        {
            // 1. Log out the calling thread
            nameof(ExplicitMultiThreadedObservation).Log();

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

        [Test]
        public void ExplicitMultiThreadedSubsciption()
        {
            // 1. Log out the calling thread
            nameof(ExplicitMultiThreadedSubsciption).Log();
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
        public void ExplicitMultiThreadedSubsciptionAndObservation()
        {
            IScheduler scheduler = new SingleThreadedScheduler("KennyScheduler");
            Console.WriteLine("Current Thread {0}", Thread.CurrentThread.ManagedThreadId);

            // 1. Create an observable
            var observable = new SimpleObservable<int>();

            // 2. Create an observer 
            IObserver<int> observer = new SimpleObserver<int>();

            var disposable = observable
                .SubscribeOn(scheduler)
                .ObserveOn(scheduler)
                .Subscribe(observer);

            Thread.Sleep(100);

            // 4. Publish 
            observable.Publish(1);

            // 4. Publish 
            observable.Publish(1);

            Thread.Sleep(100);

            // Dispose of the observer
            disposable.Dispose();
        }

        [Test]
        public void ExplicitSingleThreadedSubscription()
        {
            IScheduler scheduler = new SingleThreadedScheduler("KennysScheduler");
            Console.WriteLine("Current Thread {0}", Thread.CurrentThread.Name);

            // 1. Create an observable
            IObservable<int> observable = new SimpleObservable<int>();

            // 2. Create an observer 
            IObserver<int> observer = new SimpleObserver<int>();

            // 3. Use the reactive framework extension to 
            var subscribeOn = observable.SubscribeOn(scheduler);

            //  Subscribe on this
            var disposable = subscribeOn.Subscribe(observer);

            // 4. Publish 
            observable.Publish(1);

            // Dispose of the observer
            disposable.Dispose();
        }
    }
}