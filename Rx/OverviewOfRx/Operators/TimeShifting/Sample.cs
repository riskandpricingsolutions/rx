using System;
using System.Reactive.Linq;
using System.Threading;
using NUnit.Framework;

namespace RiskAndPricingSolutions.Rx.Expositional.OverviewOfRx.Operators.TimeShifting
{
    [TestFixture]
    public class SampleTest
    {
        [Test]
        public void Sample()
        {
            DateTime now = DateTime.Now;
            EventWaitHandle waitHandle = new AutoResetEvent(false);

            Observable
                .Interval(TimeSpan.FromSeconds(0.3))
                .Take(10)
                .Sample(TimeSpan.FromSeconds(1.0))
                .Subscribe(l => Console.WriteLine($"Delayed {l}   {(DateTime.Now - now).TotalSeconds}"), () => waitHandle.Set());

            waitHandle.WaitOne();
        }
    }
}