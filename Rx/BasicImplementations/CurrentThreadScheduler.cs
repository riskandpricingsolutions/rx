using System;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;


namespace RiskAndPricingSolutions.Rx.SharedResources.BasicImplementations
{
    /// <summary>
    /// Simple scheduler that executes on the thread the Schedule method
    /// is invoked in. 
    /// </summary>
    public class CurrentThreadScheduler : IScheduler
    {
        public IDisposable Schedule<TState>(TState state, Func<IScheduler, TState, IDisposable> action)
        {
            var disposable = action(this, state);
            return disposable;
        }

        public IDisposable Schedule<TState>(TState state, TimeSpan dueTime,
            Func<IScheduler, TState, IDisposable> action)
        {
            return Disposable.Empty;
        }

        public IDisposable Schedule<TState>(TState state, DateTimeOffset dueTime,
            Func<IScheduler, TState, IDisposable> action)
        {
            return Disposable.Empty;
        }

        public DateTimeOffset Now { get; }
    }
}
