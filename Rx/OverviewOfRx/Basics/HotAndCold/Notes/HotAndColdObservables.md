# Hot and Cold Observables
Rx makes the distinction between hot and cold observables. A cold observable only ever produces values at the point an observer subscribes and each subscriber is given its own set of data. A subscriber can never miss out on data by subscribing late. A hot observable on the other hand is always producing data irrespective of whether any observers are subscribed. With a hot observable a subscriber can miss out on earlier values if it subscribes late. We consider cold and hot observables in turn.

## Cold Observable
The basic characteristic of a cold observable is that nothing is done until a subscription is made and each subscription gets different values

*C# Sample Code*
```csharp
// We use a factory method together with Observable.Create to create
// an IObservable which delivers completely different values to each 
// Subscription. This is the basic property of a ColdObservable. 
// Nothing is delivered util a subscription is made and each 
// subscription gets a different value.
int x = 0;

// Define a factory method that when invoked directly calls OnNext
IDisposable FactMeth(IObserver<int> observer)
{
  observer.OnNext(x++);
  return Disposable.Empty;
}

// Use Observable.Create to turn our factory method into an IObservable
var observable = Observable.Create((Func<IObserver<int>, IDisposable>)FactMeth);

// Perform two different subscriptions. Each IOBserver 
// get different values to the nature of a cold observable
observable.Subscribe(i => Console.WriteLine($"A {i}"));
observable.Subscribe(i => Console.WriteLine($"B {i}"));
```
*Output*
```
A 0
B 1
```

## Connectable Observables
*C# Sample Code*
```csharp
// As per the previous sample we use a factory method together
// with Observable.Create to create an IObservable which delivers
// completely different values from each subscription. The key 
// difference is that we wrap this Observable 
// with a ConnectableObservable 
// using the Publish extension method. This extra layer allows us to
// share the values published from the originating Observable as the
// Connectable wrapper performs the multiplexing
            int x = 0;

// Define a factory method that when invoked directly calls OnNext
IDisposable FactMethod(IObserver<int> observer)
{
  observer.OnNext(x++);
  return Disposable.Empty;
}

// Use the Observable.Create to turn our factory method into an IObservable
IObservable<int> observable = Observable.Create((Func<IObserver<int>, IDisposable>)FactMethod);

// Wrap the source Observable in a Connectable observable
IConnectableObservable<int> connectableObservable = observable.Publish();

// Even though we subscribe twice the connectable observable 
// will make sure
// there is only one underlying Observable 
// with the ConnectableObservable
// providing multi-plexing
connectableObservable.Subscribe(i => Console.WriteLine($"A {i}"));
connectableObservable.Subscribe(i => Console.WriteLine($"B {i}"));

// The subscription is now carried out and multiplexed out to the
// registered observers
connectableObservable.Connect();
```

*Output*
```
A 0
B 0
```

## Hot Observables
*C# Sample Code*
```csharp
// In this example we wrap our observable in a ConnectableObservable 
// and connect it before we make any subscriptions. By doing this we are
// creating what is known as a Hot Observable. This Observable is still
// publishing values even though it has no subscriptions.
SimpleObservable<int> sourceObservable = new SimpleObservable<int>();

IConnectableObservable<int> hotObservable = sourceObservable
 .Do(i => Console.WriteLine("Source({0}) thread {1}", i, Thread.CurrentThread.ManagedThreadId))
 .Publish();

// Connecting to the IConnectableObservable causes it to subscribe on
// the sourceObservable thereby setting up a hot observable which will
// publish out even when the connectableObservable has no observers
IDisposable disposable = hotObservable.Connect();

// Tis logged via the Do call even though we have no observer on
// the connectableObservable
sourceObservable.Publish(1);

// now we subscribe on the connectableObservable
hotObservable.Subscribe(i => Console.WriteLine("OnNext({0}) thread {1}", i, Thread.CurrentThread.ManagedThreadId));

// this is now delivered to the IObserver
sourceObservable.Publish(2);

// Disposing of the connectableObservable turns off the publishing
disposable.Dispose();
sourceObservable.Publish(3);

// we can now reconnect to the same IConnectableObservable and once again
// messages are delivered
disposable = hotObservable.Connect();
sourceObservable.Publish(4);
```

*Output*
```
Subscribe on Thread "NonParallelWorker:12"
Source(1) thread 12
Source(2) thread 12
OnNext(2) thread 12
Dispose on Thread NonParallelWorker:12
Subscribe on Thread "NonParallelWorker:12"
Source(4) thread 12
OnNext(4) thread 12
```