# Producing Streams CheatSheet

## Creational Methods

### Observable.Create
##### C#
```csharp
IObservable<int> s =
 Observable.Create<int>(observer =>
 {
   observer.OnNext(1);
   observer.OnNext(2);
   observer.OnNext(3);
   observer.OnCompleted();
   return Disposable.Empty;
 });

 s.Subscribe(i => 
	WriteLine($"OnNext({i})"), 
		() => WriteLine("OnCompleted"));
```

### Observable.Empty
Returns an empty stream which simply invokes OnCompleted to end the sequence
##### C#

```csharp
IObservable<int> s =
  Observable.Empty<int>();

s.Subscribe(i => WriteLine($"OnNext({i})"),
	() => WriteLine("OnCompleted"));
```
### Observable.Return(T)
Returns a stream consisting of a single value. 
##### C#
```csharp
Observable.Return(1)
 .Subscribe(WriteLine, () => WriteLine("OnCompleted"));
```

### Observable.Throw
Returns a stream which calls OnError and then returns
##### C#
```csharp
IObservable<int> s =
  Observable.Throw<int>(new Exception("An exception"));
s.Subscribe(i => WriteLine($"OnNext({i})"),
  exception => WriteLine("OnException"),
  () => WriteLine("OnCompleted"));
      
```
## Unfold Methods
An unfold can be used to produce a possibly infinite sequene. Generate is the root Rx unfold method. The other unfold methods such as Range, Interval and Timer could be produced using Generate. 
### Observable.Generate
Supports the creation of more complex sequences of event. 
##### C#
```csharp
IObservable<int> s =
 Observable.Generate(0, i => i < 5, i => i + 1, i => i);

s.Subscribe(i => WriteLine($"OnNext({i})"),
  exception => WriteLine("OnException"),
  () => WriteLine("OnCompleted"));
```
##### Marble


### Observable.Range(int,int)
Returns an observable over a sequence of consecutive integers
##### C#
```csharp
Observable.Range(1,3)
 .Subscribe(WriteLine, () => WriteLine("OnCompleted"));
```

### Observable.Interval(TimeSpan)
Produces incrementally increasing integers. The gap between elements is defined by the given Timespan

##### C#
```csharp
Observable
 .Interval(TimeSpan.FromSeconds(0.25))
 .Take(4)
 .ObserveOn(Scheduler.Default)
 .Subscribe(l => WriteLine($"{l}"),
   () => tcs.SetResult(null));
```

##### Marble


### Observable.Timer(TimeSpan)
Takes a timespan and creates a stream of one value. The single value is delivered after the specified TimeSpan has elapsed

##### C#
```csharp
TaskCompletionSource<string> tcs = new TaskCompletionSource<string>();

Observable
 .Timer(TimeSpan.FromSeconds(1))
 .ObserveOn(Scheduler.Default)
 .Subscribe(l => WriteLine($"{l}"),() => tcs.SetResult(null));

```

### Observable.Timer(TimeSpan,TimeSpan)
Takes two TimeSpan objects. The first determines how much time should be allowed to elapse before the first element is delivered. The second TimeSpan determines the interval between subsequent events. 
##### C#
```csharp
TaskCompletionSource<string> tcs = new TaskCompletionSource<string>();

Observable
 .Timer(TimeSpan.FromSeconds(2),TimeSpan.FromSeconds(0.1))
 .Take(4)
 .ObserveOn(Scheduler.Default)
 .Subscribe(l => WriteLine($"{l}"),() => tcs.SetResult(null));

```

## Transitioning from other API's

We can use the methods of Rx to create Observables form other .NET types. 
### Observable.Start(Action)
Creates a single value observable from an Action delegate
##### C#
```csharp
Action a = () => { };
Observable
  .Start(a)
  .Subscribe(unit => WriteLine($"OnNext({unit})"),
                    () => WriteLine("OnCompleted"));

```

### Observable.Start(Func<T>)
Creates a single value observable from a Func<T>. The value is the value returned by the Func<T>
##### C#
```csharp
Action a = () => { };
Observable
  .Start(a)
  .Subscribe(unit => WriteLine($"OnNext({unit})"),
                    () => WriteLine("OnCompleted"));

```

### Observable.FromEventPattern
Creates Observables from standard .NET events
### C#
```csharp
IObservable<EventPattern<EventArgs>> obs
 = Observable
    .FromEventPattern<EventArgs>(h => MyEvent += h, h => MyEvent -= h);

obs.Subscribe(x => WriteLine(x.EventArgs));
MyEvent?.Invoke(this,new EventArgs());
```

### Task.ToObservable
Creates Observables from standard tasks
### C#
```csharp
TaskCompletionSource<string> tcs = new TaskCompletionSource<string>();
 tcs.SetResult("Value");
tcs
 .Task
 .ToObservable()
 .Subscribe(x => WriteLine($"OnNext({x})"), () => WriteLine("OnCompleted"));
WriteLine("Subscribed to already completed task\n");

Task
 .Run(() => "Value")
 .ToObservable()
 .Subscribe(x => WriteLine($"OnNext({x})"), () => WriteLine("OnCompleted"));
WriteLine("Subscribed to running task");
```

### IEnumerable<T>.ToObservable
### C#
```csharp
Enumerable
 .Range(0,3)
 .ToObservable()
 .Subscribe(x => WriteLine($"OnNext({x})"), () => WriteLine("OnCompleted"));
```