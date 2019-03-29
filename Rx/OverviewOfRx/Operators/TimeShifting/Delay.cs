using System;
using System.Reactive.Linq;
using System.Threading;
using NUnit.Framework;

namespace RiskAndPricingSolutions.Rx.Expositional.OverviewOfRx.Operators.TimeShifting
{
    [TestFixture]
    public class Delay
    {
        [Test]
        public void DelayTest()
        {
            DateTime now = DateTime.Now;
            EventWaitHandle latch = new AutoResetEvent(false);

            var source = Observable.Interval(TimeSpan.FromSeconds(0.5)).Take(4);
            var delays = source.Delay(TimeSpan.FromSeconds(1.0));

            source.Subscribe(l => Console.WriteLine($"Original {l}   {(DateTime.Now - now).TotalSeconds}"));
            delays.Subscribe(l => Console.WriteLine($"Delayed {l}   {(DateTime.Now - now).TotalSeconds}"), () => latch.Set());

            latch.WaitOne();
        }
    }
}