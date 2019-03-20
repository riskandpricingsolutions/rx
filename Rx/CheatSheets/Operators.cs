using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using NUnit.Framework;
using RiskAndPricingSolutions.Rx.SharedResources.BasicImplementations;
using static System.Console;

namespace CheatSheets
{
    [TestFixture]
    public class Operators
    {
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
            Observable.Range(0,3)
                .Subscribe(WriteLine, () => WriteLine("OnCompleted"));

            // Observable.Range(int)
            Observable.Range(0, 2)
                .Repeat(2)
                .Subscribe(WriteLine, () => WriteLine("OnCompleted"));

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
