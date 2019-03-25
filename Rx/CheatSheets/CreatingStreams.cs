using System;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;
using static System.Console;

namespace RiskAndPricingSolutions.Rx.CheatSheets
{
    [TestFixture]
    public class CreatingStreams
    {
        [Test]
        public void Create()
        {
            IObservable<int> s =
                Observable.Create<int>(observer =>
                {
                    observer.OnNext(1);
                    observer.OnNext(2);
                    observer.OnNext(3);
                    observer.OnCompleted();
                    return Disposable.Empty;
                });

            s.Subscribe(i => WriteLine($"OnNext({i})"),
                () => WriteLine("OnCompleted"));
        }

        [Test]
        public void Empty()
        {
            IObservable<int> s =
                Observable.Empty<int>();

            s.Subscribe(i => WriteLine($"OnNext({i})"),
                () => WriteLine("OnCompleted"));
        }

        [Test]
        public void Return()
        {
            IObservable<int> s =
                Observable.Return(5);

            s.Subscribe(i => WriteLine($"OnNext({i})"),
                () => WriteLine("OnCompleted"));
        }

        [Test]
        public void Throw()
        {
            IObservable<int> s =
                Observable.Throw<int>(new Exception("An exception"));

            s.Subscribe(i => WriteLine($"OnNext({i})"),
                exception => WriteLine("OnException"),
                () => WriteLine("OnCompleted"));
        }

        [Test]
        public void Generate()
        {
            IObservable<int> s =
                Observable.Generate(0, i => i < 5, i => i + 1, i => i);

            s.Subscribe(i => WriteLine($"OnNext({i})"),
                exception => WriteLine("OnException"),
                () => WriteLine("OnCompleted"));
        }

        [Test]
        public Task Interval()
        {
            TaskCompletionSource<string> tcs = new TaskCompletionSource<string>();

            Observable
                .Interval(TimeSpan.FromSeconds(0.25))
                .Take(4)
                .ObserveOn(Scheduler.Default)
                .Subscribe(l => WriteLine($"{l}"),
                    () => tcs.SetResult(null));

            return tcs.Task;
        }

        [Test]
        public Task Timer()
        {
            TaskCompletionSource<string> tcs = new TaskCompletionSource<string>();

            Observable
                .Timer(TimeSpan.FromSeconds(1))
                .ObserveOn(Scheduler.Default)
                .Subscribe(l => WriteLine($"{l}"),
                    () => tcs.SetResult(null));

            return tcs.Task;
        }

        [Test]
        public Task Timer2()
        {
            TaskCompletionSource<string> tcs = new TaskCompletionSource<string>();

            Observable
                .Timer(TimeSpan.FromSeconds(2), TimeSpan.FromSeconds(0.1))
                .Take(4)
                .ObserveOn(Scheduler.Default)
                .Subscribe(l => WriteLine($"{l}"),
                    () => tcs.SetResult(null));

            return tcs.Task;
        }

        [Test]
        public void Generate2()
        {
            AutoResetEvent h = new AutoResetEvent(false);

            WriteLine("Generate");
            Observable.Generate(
                    0,
                    i => i < 3,
                    i => ++i,
                    i => $"Value {i}",
                    i => TimeSpan.FromSeconds(i))
                .SubscribeOn(DefaultScheduler.Instance)
                .ObserveOn(DefaultScheduler.Instance)
                .Subscribe(WriteLine, () => h.Set());

            h.WaitOne();
        }

        [Test]
        public void StartFromAction()
        {
            Action a = () => { };
            Observable
                .Start(a)
                .Subscribe(unit => WriteLine($"OnNext({unit})"),
                    () => WriteLine("OnCompleted"));
        }

        [Test]
        public void StartFromFunc()
        {
            Func<int> a = () => 5;
            Observable
                .Start(a)
                .Subscribe(unit => WriteLine($"OnNext({unit})"),
                    () => WriteLine("OnCompleted"));
        }
    }
}