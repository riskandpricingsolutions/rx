using System;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using NUnit.Framework;
using static System.Console;

namespace RiskAndPricingSolutions.Rx.Expositional.OverviewOfRx.Operators.Inspecting
{
    [TestFixture]
    public class AnyTest
    {
        [Test]
        public void AnyOnEmptyStreamReturnsFalse()
        {
            IObservable<bool> result = Observable.Any(Observable
                .Empty<string>());

            result.Subscribe(WriteLine);
        }

        [Test]
        public void AnyOnNonEmptyStreamReturnsTrue()
        {
            IObservable<bool> result = Observable.Any(Observable
                    .Return("Hello"));

            result.Subscribe(WriteLine);
        }

        [Test]
        public void
            AnyWithExceptionPassesItOn()
        {
            Observable
                .Throw<string>(new Exception("An exception"))
                .Any()
                .Subscribe(WriteLine, x => WriteLine(x));
        }

        [Test]
        public void IfAnyElementMatchesPredicateReturnsTrue()
        {
            IObservable<bool> result = Observable.Any(Observable
                .Range(0, 3), x => x == 2);
            
            result.Subscribe(WriteLine);
        }

        [Test]
        public void IfNoElementsMatchPredicateReturnsFalse()
        {
            IObservable<bool> result = Observable.Any(Observable
                    .Range(0, 3), x => x > 3);

            result.Subscribe(WriteLine);
        }
    }

    public static class MyAny
    {
        public static IObservable<bool> Any<TElement>(this IObservable<TElement> source)
        {
            return Observable.Create<bool>(observer =>
            {
                source.Subscribe(x =>
                    {
                        observer.OnNext(true);
                        observer.OnCompleted();
                    },
                    observer.OnError,
                    () =>
                    {
                        observer.OnNext(false);
                        observer.OnCompleted();
                    });
                return Disposable.Empty;
            });
        }

        public static IObservable<bool> Any<TElement>(this IObservable<TElement> source, Func<TElement, bool> predicate)
        {
            return source.Where(predicate).Any();
        }
    }
}