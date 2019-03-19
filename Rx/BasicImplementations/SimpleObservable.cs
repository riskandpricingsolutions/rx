using System;
using System.Collections.Generic;
using System.Threading;

namespace RiskAndPricingSolutions.Rx.SharedResources.BasicImplementations
{
    public class SimpleObservable<T> : IObservable<T>
    {
        public IDisposable Subscribe(IObserver<T> observer)
        {
            Console.WriteLine($"Subscribe on Thread \"{Thread.CurrentThread.Name}:{Thread.CurrentThread.ManagedThreadId}\"");
            lock (_observers)
                _observers.Add(observer);

            return new ActionDisposable(() =>
            {
                Console.WriteLine($"Dispose on Thread {Thread.CurrentThread.Name}:{Thread.CurrentThread.ManagedThreadId}");
                lock (_observers)
                    _observers.Remove(observer);
            });
        }

        public void Publish(T val)
        {
            List<IObserver<T>> copy;
            lock (_observers)
                copy = new List<IObserver<T>>(_observers);

            copy.ForEach(observer => observer.OnNext(val));
        }

        public void Complete()
        {
            List<IObserver<T>> copy;
            lock (_observers)
                copy = new List<IObserver<T>>(_observers);

            copy.ForEach(observer => observer.OnCompleted());
        }

        private readonly List<IObserver<T>> _observers = new List<IObserver<T>>();
    }
}
