using System;
using System.Collections.Generic;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using NUnit.Framework;

namespace RiskAndPricingSolutions.Articles.Rx.Basics
{
    public class MyObservable<T> : IObservable<T>
    {
        private readonly List<IObserver<T>> _observers = new List<IObserver<T>>();

        public IDisposable Subscribe(IObserver<T> observer)
        {
            _observers.Add(observer);
            return Disposable.Empty;
        }

        public void Publish(T m) => _observers.ForEach(obs => obs.OnNext(m));
    }

    public class MyObserver<T> : IObserver<T>
    {
        public void OnNext(T value) => Console.WriteLine(value);
        public void OnError(Exception error) => Console.WriteLine("Error");
        public void OnCompleted() => Console.WriteLine("Completed");
    }

    [TestFixture]
    public class ObservableSubscribe
    {
        [Test]
        public void HandCrafted()
        {
            MyObservable<int> observable = new MyObservable<int>();
            MyObserver<int> observer = new MyObserver<int>();
            IDisposable disposable = observable.Subscribe(observer);

            observable.Publish(1);
            disposable.Dispose();
            observable.Publish(2);
        }

        [Test]
        public void UsingObservableSubscribe()
        {
            MyObservable<int> observable = new MyObservable<int>();
            MyObserver<int> observer = new MyObserver<int>();

            var decorator = Observable.Create<int>(o => observable.Subscribe(o));
            var disposable = decorator.Subscribe(observer);

            observable.Publish(1);
            disposable.Dispose();
            observable.Publish(2);
        }
    }
}
