using System;
using System.Reactive.Joins;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using NUnit.Framework;
using static System.Console;

namespace RiskAndPricingSolutions.Rx.CheatSheets
{
    [TestFixture]
    public class CombiningStreams
    {
        [Test]
        public void Concat()
        {
            Observable.Range(0, 2).Concat(Observable.Range(5, 2))
                .Subscribe(WriteLine);
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

            Observable.Amb(a, b)
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
        public void CombineLatest()
        {
            Subject<string> a = new Subject<string>();
            Subject<string> b = new Subject<string>();

            Observable.CombineLatest(a, b, (s, s1) => $"({s},{s1})")
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
            IObservable<int> b = Observable.Range(1, 3).Select(x => x * 2);
            IObservable<int> c = Observable.Range(1, 3).Select(x => x * 3);

            a
                .Zip(b, (a1, b1) => (a1, b1))
                .Zip(c, (tuple, c1) => (tuple.Item1, tuple.Item2, c1))
                .Subscribe(res => WriteLine(res));
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
    }
}