﻿using System;
using System.Reactive.Linq;
using System.Threading;
using NUnit.Framework;
using RiskAndPricingSolutions.Rx.SharedResources.BasicImplementations;
using static System.Console;

namespace RiskAndPricingSolutions.Rx.CheatSheets
{
    [TestFixture]
    public class TimeShiftingStreams
    {
        [Test]
        public void BufferWithCount()
        {
            Observable
                .Range(0, 5)
                .Buffer(2)
                .Subscribe(ints => WriteLine(string.Join(",", ints)));
        }

        [Test]
        public void BufferWithCountAndSkip1()
        {
            Observable.Range(1, 5)
                .Buffer(2, 3)
                .Subscribe(ints => WriteLine(string.Join(",", ints)));
        }

        [Test]
        public void BufferWithCountAndSkip2()
        {
            Observable.Range(1, 5)
                .Buffer(3, 2)
                .Subscribe(ints => WriteLine(string.Join(",", ints)));
        }

        [Test]
        public void BufferWithTimeSpan()
        {
            EventWaitHandle ewh = new AutoResetEvent(false);
            Observable
                .Interval(TimeSpan.FromSeconds(0.3))
                .Take(6)
                .Buffer(TimeSpan.FromSeconds(1.0))
                .Subscribe(ints => WriteLine(string.Join(",", ints)),()=>ewh.Set());

            ewh.WaitOne();
        }

        [Test]
        public void BufferWithTimeSpanAndTimeShift1()
        {
            // Start a new buffer every 0.5 seconds and each buffer is 1.0 second long
            // to give overlapping behaviour
            EventWaitHandle ewh = new AutoResetEvent(false);
            Observable
                .Interval(TimeSpan.FromSeconds(0.3))
                .Take(6)
                .Buffer(TimeSpan.FromSeconds(1.0), TimeSpan.FromSeconds(0.4))
                .Subscribe(ints => WriteLine(string.Join(",", ints)), () => ewh.Set());

            ewh.WaitOne();
        }



        [Test]
        public void BufferWithClosingSelector()
        {
            EventWaitHandle latch = new AutoResetEvent(false);

            var obs = Observable
                .Interval(TimeSpan.FromSeconds(0.3))
                .Take(10);

            var closing = Observable
                .Interval(TimeSpan.FromSeconds(1.0))
                .Take(2);

            obs
                .Buffer(closing)
                .Subscribe(ints => WriteLine(string.Join(",", ints)), () => latch.Set());

            latch.WaitOne();
        }

        [Test]
        public void BufferWithOpeningAndClosingSelectors()
        {
            EventWaitHandle latch = new AutoResetEvent(false);

            var obs = Observable
                .Interval(TimeSpan.FromSeconds(0.3))
                .Take(10);

            var opening = Observable
                .Interval(TimeSpan.FromSeconds(0.7))
                .Take(2);

            var closing = Observable
                .Timer(TimeSpan.FromSeconds(0.5))
                .Take(2);

            obs
                .Buffer(opening, i => closing)
                .Subscribe(ints => WriteLine($"({string.Join(",", ints)})"), () => latch.Set());

            latch.WaitOne();
        }


        [Test]
        public void BufferWithClosingSelectorShowingTimings()
        {
            DateTime now = DateTime.Now;
            EventWaitHandle latch = new AutoResetEvent(false);

            var obs = Observable
                .Interval(TimeSpan.FromSeconds(0.3))
                .Do(l => WriteLine($"Value: {(DateTime.Now-now).TotalSeconds}"))
                .Take(10);

            var closing = Observable
                .Interval(TimeSpan.FromSeconds(1.0))
                .Do(l => WriteLine($"Closing: Signal {(DateTime.Now - now).TotalSeconds}"))
                .Take(2);

            obs
                .Buffer(closing)

                .Subscribe(ints => WriteLine($"Buffer: {string.Join(",", ints)} {(DateTime.Now - now).TotalSeconds}"),()=>latch.Set());

            latch.WaitOne();
        }

        [Test]
        public void BufferWithOpeningAndClosingSelectorShowingTimings()
        {
            DateTime now = DateTime.Now;
            EventWaitHandle latch = new AutoResetEvent(false);

            var obs = Observable
                .Interval(TimeSpan.FromSeconds(0.3))
                .Do(l => WriteLine($"Value({l}): {(DateTime.Now - now).TotalSeconds}"))
                .Take(10);

            var opening = Observable
                .Interval(TimeSpan.FromSeconds(0.7))
                .Do(l => WriteLine($"Opening: Signal {(DateTime.Now - now).TotalSeconds}"))
                .Take(2);

            var closing = Observable
                .Timer(TimeSpan.FromSeconds(0.5))
                .Do(l => WriteLine($"Closing: Signal {(DateTime.Now - now).TotalSeconds}"))
                .Take(2);

            obs
                .Buffer(opening, i => closing)
                .Subscribe(ints => WriteLine($"({string.Join(",", ints)})"), () => latch.Set());

            latch.WaitOne();
        }
    }
}