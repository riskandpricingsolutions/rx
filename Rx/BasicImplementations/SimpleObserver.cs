using System;
using System.Threading;

namespace RiskAndPricingSolutions.Rx.SharedResources.BasicImplementations
{
    /// <summary>
    /// An expositional implementation of the IObserver interface
    /// that simply logs messages along with the thread they are 
    /// executed on
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class SimpleObserver<T> : IObserver<T>
    {
        public void OnNext(T value) =>
            Console.WriteLine(
                $"OnNext({value}) thread {Thread.CurrentThread.Name}:{Thread.CurrentThread.ManagedThreadId}");

        public void OnError(Exception error) =>
            Console.WriteLine(
                $"OnError({error}) thread {Thread.CurrentThread.Name}:{Thread.CurrentThread.ManagedThreadId}");

        public void OnCompleted() =>
            Console.WriteLine(
                $"OnCompleted() thread {Thread.CurrentThread.Name}:{Thread.CurrentThread.ManagedThreadId}");
    }
}
