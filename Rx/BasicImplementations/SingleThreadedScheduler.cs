using System;
using System.Collections.Concurrent;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using System.Threading;

namespace RiskAndPricingSolutions.Rx.SharedResources.BasicImplementations
{
    /// <summary>
    /// A Scheduler which executes everything on a single thread which 
    /// it creates on construction
    /// </summary>
    public class SingleThreadedScheduler : IScheduler
    {
        public SingleThreadedScheduler(string threadName)
        {
            Thread t = new Thread(() =>
                {
                    while (true)
                    {
                        try
                        {
                            Action a = _queue.Take();
                            a();
                        }
                        catch (Exception)
                        {
                        }
                    }
                })
                { Name = threadName, IsBackground = false };
            t.Start();
        }

        public IDisposable Schedule<TState>(TState state, Func<IScheduler, TState, IDisposable> action)
        {
            var d = new SingleAssignmentDisposable();

            _queue.TryAdd(() =>
            {
                if (d.IsDisposed)
                    return;

                d.Disposable = action(this, state);
            });

            return d;
        }

        public IDisposable Schedule<TState>(TState state, TimeSpan dueTime, Func<IScheduler, TState, IDisposable> action)
            => Disposable.Empty;

        public IDisposable Schedule<TState>(TState state, DateTimeOffset dueTime,
            Func<IScheduler, TState, IDisposable> action) => Disposable.Empty;


        public DateTimeOffset Now { get; }

        private readonly BlockingCollection<Action> _queue
            = new BlockingCollection<Action>(new ConcurrentQueue<Action>());
    }
}
