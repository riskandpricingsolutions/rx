using System;
using System.Reactive.Linq;
using System.Threading;
using NUnit.Framework;

namespace RiskAndPricingSolutions.Rx.Expositional.OverviewOfRx.Operators.TimeShifting
{
    [TestFixture]
    public class ThrottleTest
    {
        [Test]
        public void TestThrottleTest()
        {
            DateTime now = DateTime.Now;
            EventWaitHandle waitHandle = new AutoResetEvent(false);

            Observable
                .Interval(TimeSpan.FromSeconds(1.0))
                .Take(4)
                .Throttle(TimeSpan.FromSeconds(0.7))
                .Subscribe(l => Console.WriteLine($"Delayed {l}   {(DateTime.Now - now).TotalSeconds}"), () => waitHandle.Set());

            waitHandle.WaitOne();
        }

        [Test]
        public void TestThrottleTest2()
        {
            DateTime now = DateTime.Now;
            EventWaitHandle waitHandle = new AutoResetEvent(false);

            Observable
                .Interval(TimeSpan.FromSeconds(1.0))
                .Take(4)
                .Throttle(TimeSpan.FromSeconds(1.5))
                .Subscribe(l => Console.WriteLine($"Delayed {l}   {(DateTime.Now - now).TotalSeconds}"), () => waitHandle.Set());

            waitHandle.WaitOne();
        }

        [Test]
        public void TestThrottleTest3()
        {
            DateTime now = DateTime.Now;
            EventWaitHandle waitHandle = new AutoResetEvent(false);

            Observable
                .Timer(TimeSpan.FromSeconds(0.01)).Select(x=>1L)
                .Concat(Observable
                    .Timer(TimeSpan.FromSeconds(0.1))).Select(x=>2L)
                .Concat(Observable
                    .Timer(TimeSpan.FromSeconds(0.1))).Select(x=>3L)
                .Concat(Observable
                    .Timer(TimeSpan.FromSeconds(0.1))).Select(x=>4L)
                    .Do(l => Console.WriteLine($"Delayed {l}   {(DateTime.Now - now).TotalSeconds}"))
                .Throttle(TimeSpan.FromSeconds(1.0))
                .Subscribe(l => Console.WriteLine($"Result {l}   {(DateTime.Now - now).TotalSeconds}"), () => waitHandle.Set());

            waitHandle.WaitOne();
        }
    }
}