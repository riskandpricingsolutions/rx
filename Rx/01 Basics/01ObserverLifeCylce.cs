using System;
using NUnit.Framework;
using RiskAndPricingSolutions.Rx.SharedResources.BasicImplementations;

namespace RiskAndPricingSolutions.Articles.Rx.Basics
{
    [TestFixture]
    public class ObserverLifeCycle
    {
        [Test]
        public void TheLifeCycleOfAnObserver()
        {
            // (1) Create an instance of an Observable
            SimpleObservable<int> observable = new SimpleObservable<int>();

            // (2) Create an instance of the IObserver
            IObserver<int> observer = new SimpleObserver<int>();

            // (3) Subscribe the observer to the observable
            IDisposable disposable = observable.Subscribe(observer);

            // (4) Observer delivers some events
            observable.Publish(1);
            observable.Publish(2);

            // (5) After disposal the Observer no longer delivers
            // events to the disposed observer
            disposable.Dispose();
            observable.Publish(3);
        }
    }
}