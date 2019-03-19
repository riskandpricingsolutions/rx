using System;

namespace RiskAndPricingSolutions.Rx.SharedResources.BasicImplementations
{
    public class DelegateBasedObserver<T> : IObserver<T>
    {
        public DelegateBasedObserver(Action<T> nextAction)
        {
            _nextAction = nextAction;
        }

        public DelegateBasedObserver(Action<T> nextAction,
            Action completedAction) : this(nextAction)
        {
            _completedAction = completedAction;
        }

        public DelegateBasedObserver(Action<T> nextAction,
            Action<Exception> exceptAction, Action completedAction) :
            this(nextAction, completedAction)
        {
            _exceptAction = exceptAction;
        }

        public void OnNext(T value)
        {
            _nextAction?.Invoke(value);
        }

        public void OnError(Exception error)
        {
            _exceptAction?.Invoke(error);
        }

        public void OnCompleted()
        {
            _completedAction?.Invoke();
        }

        private readonly Action<T> _nextAction;
        private readonly Action _completedAction;
        private readonly Action<Exception> _exceptAction;
    }
}
