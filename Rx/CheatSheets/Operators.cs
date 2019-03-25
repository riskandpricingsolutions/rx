using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using System.Reactive.Joins;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;
using RiskAndPricingSolutions.Rx.SharedResources.BasicImplementations;
using static System.Console;

namespace CheatSheets
{
    [TestFixture]
    public class Operators
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
                .Timer(TimeSpan.FromSeconds(2),TimeSpan.FromSeconds(0.1))
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

        [Test]
        public void OperatorCheatSheet()
        {
            // Observable.Return
            // Return an observable which consists of one value
            // After the single value is delivered the OnCompleted is invoked
            Observable.Return(1)
                .Subscribe(WriteLine, () => WriteLine("OnCompleted"));

            // Observable.Range(int,int)
            // Return an observable which consists of n consecutive iteger values
            // Finally OnCompleted is invoked
            Observable.Range(1,3)
                .Subscribe(WriteLine, () => WriteLine("OnCompleted"));


            // Observable.Range(int)
            Observable.Range(0, 2)
                .Repeat(2)
                .Subscribe(WriteLine, () => WriteLine("OnCompleted\n"));

        }



        [Test]
        public void Scan()
        {
            WriteLine("Scan");
            Observable.Range(1, 3)
                .Scan(0,(cum, i1) => cum+i1)
                .Subscribe(WriteLine, () => WriteLine("OnCompleted\n"));
        }

        [Test]
        public void Repeat()
        {
            // Observable.Repear(TResult,int)
            // Return an observable which repeats the given 
            //  value the specified number of times
            WriteLine("Repeat(TRresult,int)");
            Observable.Repeat(2, 3)
                .Subscribe(WriteLine, () => WriteLine("OnCompleted\n"));
        }

        [Test]
        public void StartsWith()
        {
            Observable
                .Range(10, 2)
                .StartWith(8, 9)
                .Subscribe(WriteLine);
        }

        [Test]
        public void Amb()
        {
            Subject<string> a = new Subject<string>();
            Subject<string> b = new Subject<string>();

            Observable.Amb(a,b)
                .Subscribe(WriteLine);

            a.OnNext("a");
            b.OnNext("1");
            a.OnNext("b");
            b.OnNext("2");
        }

        [Test]
        public void Merge()
        {
            Subject<string> a = new Subject<string>();
            Subject<string> b = new Subject<string>();

            Observable.Merge(a, b)
                .Subscribe(WriteLine);

            a.OnNext("a");
            b.OnNext("1");
            a.OnNext("b");
            b.OnNext("2");
        }

        [Test]
        public void Switch()
        {
            Subject<string> a = new Subject<string>();
            Subject<string> b = new Subject<string>();

            Subject<Subject<string>> master = new Subject<Subject<string>>();

            master.Switch()
                .Subscribe(WriteLine);

            master.OnNext(a);
            a.OnNext("a");
            a.OnNext("b");
            master.OnNext(b);
            b.OnNext("1");
            b.OnNext("2");
            a.OnNext("c");
        }

        [Test]
        public void Concat()
        {
            Observable.Range(0, 2).Concat(Observable.Range(5, 2))
                .Subscribe(WriteLine);
        }

        [Test]
        public void CombineLatest()
        {
            Subject<string> a = new Subject<string>();
            Subject<string> b = new Subject<string>();

            Observable.CombineLatest(a,b,(s, s1) => $"({s},{s1})")
                .Subscribe(WriteLine);

            a.OnNext("a");
            a.OnNext("b");
            b.OnNext("1");
            b.OnNext("2");
            a.OnNext("c");
        }

        [Test]
        public void Zip()
        {
            Subject<string> a = new Subject<string>();
            Subject<string> b = new Subject<string>();

            Observable.Zip(a, b, (s, s1) => $"({s},{s1})")
                .Subscribe(WriteLine);

            a.OnNext("a");
            a.OnNext("b");
            b.OnNext("1");
            b.OnNext("2");
            a.OnNext("c");
        }

        [Test]
        public void ZipChaining()
        {
            // We can Zip more than two streams together by chanining
            // multiple Zips as below. We can also use And/Then/When
            IObservable<int> a = Observable.Range(1, 3);
            IObservable<int> b = Observable.Range(1, 3).Select(x=>x*2 );
            IObservable<int> c = Observable.Range(1, 3).Select(x=>x*3 );

            a
                .Zip(b, (a1, b1) => (a1, b1))
                .Zip(c, (tuple, c1) => (tuple.Item1, tuple.Item2, c1))
                .Subscribe(res=>WriteLine(res));
        }


        [Test]
        public void AndThenWhen()
        {
            // Rather than multiple zips we can use AndThen
            IObservable<int> a = Observable.Range(1, 3);
            IObservable<int> b = Observable.Range(1, 3).Select(x => x * 2);
            IObservable<int> c = Observable.Range(1, 3).Select(x => x * 3);

            Observable
                .When(a
                    .And(b)
                    .And(c)
                    .Then((x, y, z) => (x, y, z)))
                .Subscribe(x => WriteLine(x));

            // Verbose form to show what is happening
            Pattern<int, int> pattern1 = a.And(b);
            Pattern<int, int, int> pattern2 = pattern1.And(c);
            Plan<(int, int, int)> then = pattern2.Then((i, i1, i2) => (i, i1, i2));
            IObservable<(int, int, int)> observable = Observable.When(then);
            observable.Subscribe(x => WriteLine(x));
        }

        [Test]
        public void SelectMany()
        {
            Subject<int>[] subs = new Subject<int>[]
            {
                new Subject<int>(), 
                new Subject<int>() 
            };

            Observable
                .Range(0, 2)
                .SelectMany(i => subs[i])
                .Subscribe(WriteLine);

            subs[0].OnNext(1);
            subs[1].OnNext(2);
            subs[0].OnNext(3);
            subs[1].OnNext(4);
        }

        [Test]
        public void BufferWithCount()
        {
            SimpleObservable<int> myObservable = new SimpleObservable<int>();
            IObservable<IList<int>> buffered = myObservable.Buffer(2);

            buffered.Subscribe(ints => Console.WriteLine(string.Join(",", ints)));

            myObservable.Publish(1);
            myObservable.Publish(2);
            myObservable.Publish(3);
            myObservable.Publish(4);
            myObservable.Publish(5);
            myObservable.Complete();
        }

        [Test]
        public void CountAndSkip()
        {
            SimpleObservable<int> sourceObs = new SimpleObservable<int>();

            // Buffers will be started on the 1st, 4th, 7th element. Each buffer 
            // will have two elements. 
            IObservable<IList<int>> buffer = sourceObs.Buffer(2, 3);
            buffer.Subscribe(ints => Console.WriteLine(string.Join(",", ints)));

            sourceObs.Publish(1);
            sourceObs.Publish(2);
            sourceObs.Publish(3);
            sourceObs.Publish(4);
            sourceObs.Publish(5);
            sourceObs.Publish(6);
        }

        [Test]
        public void BufferClosingSelector()
        {
            SimpleObservable<int> myObservable = new SimpleObservable<int>();
            SimpleObservable<string> closingObs = new SimpleObservable<string>();

            IObservable<string> ClosingSelector() => closingObs;
            IObservable<IList<int>> buffered = myObservable.Buffer(ClosingSelector);
            buffered.Subscribe(ints => Console.WriteLine(string.Join(",", ints)));

            myObservable.Publish(1);
            closingObs.Publish("Close Second");
            myObservable.Publish(2);
            myObservable.Publish(3);
            myObservable.Publish(4);
            closingObs.Publish("Close Second");
            myObservable.Publish(5);
            myObservable.Complete();
        }

        [Test]
        public void BufferOpeningClosingSelector()
        {
            SimpleObservable<int> myObservable = new SimpleObservable<int>();
            SimpleObservable<string> closingObs = new SimpleObservable<string>();
            SimpleObservable<string> openingObs = new SimpleObservable<string>();
            IObservable<string> ClosingSelector(string op) => closingObs;


            IObservable<IList<int>> buffered = myObservable.Buffer(openingObs, ClosingSelector);
            buffered.Subscribe(ints => Console.WriteLine(string.Join(",", ints)));

            myObservable.Publish(1);
            myObservable.Publish(2);
            openingObs.Publish("Open");
            myObservable.Publish(3);
            myObservable.Publish(4);
            closingObs.Publish("Close");
            myObservable.Publish(5);
            myObservable.Complete();
        }
    }
}
