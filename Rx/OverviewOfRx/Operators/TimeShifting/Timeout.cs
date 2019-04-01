using System;
using System.Reactive.Linq;
using System.Threading;
using NUnit.Framework;
using static System.Console;

namespace RiskAndPricingSolutions.Rx.Expositional.OverviewOfRx.Operators.TimeShifting
{
    [TestFixture]
    public class TimeoutTest
    {
        [Test]
        public void TimeOutWithTimeSpan()
        {
            Observable
                .Interval(TimeSpan.FromSeconds(0.1))
                .Take(5)
                .Concat(Observable.Interval(TimeSpan.FromSeconds(1.0)).Take(2))
                .Timeout(TimeSpan.FromSeconds(0.7))
                .Subscribe(WriteLine, WriteLine, () => WriteLine("OnCompleted"));

            Thread.Sleep(3000);
        }

        [Test]
        public void TimeOutWithTimeSpanAndAlternativeSequence()
        {
            IObservable<long> observable = Observable.Timer(TimeSpan.FromSeconds(0.7));
            IObservable<long> range = Observable.Range(20, 3).Select(x=>(long)x);

            Observable
                .Interval(TimeSpan.FromSeconds(0.1))
                .Take(5)
                .Concat(Observable.Interval(TimeSpan.FromSeconds(1.0)).Take(2))
                .Timeout(Observable.Timer(TimeSpan.FromSeconds(0.7)),l =>observable,range)
                .Subscribe(WriteLine, WriteLine, () => WriteLine("OnCompleted"));

            Thread.Sleep(3000);
        }
    }
}