using System;
using System.Reactive.Linq;
using NUnit.Framework;
using static System.Console;

namespace RiskAndPricingSolutions.Rx.CheatSheets
{
    [TestFixture]
    public class AggregatingStreams
    {
        [Test]
        public void Scan()
        {
            WriteLine("Scan");
            Observable.Range(1, 3)
                .Scan(0, (cum, i1) => cum + i1)
                .Subscribe(WriteLine, () => WriteLine("OnCompleted\n"));
        }
    }
}