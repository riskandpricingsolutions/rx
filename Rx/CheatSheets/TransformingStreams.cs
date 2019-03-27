using System;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using NUnit.Framework;
using static System.Console;

namespace RiskAndPricingSolutions.Rx.CheatSheets
{
    [TestFixture]
    public class TransformingStreams
    {
        [Test]
        public void SelectMany()
        {
            Subject<int>[] subs =
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
    }
}