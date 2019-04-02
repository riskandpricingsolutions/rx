# Scheduling
## Default Scheduling
```csharp
var observable = new SimpleObservable<int>();

// 2. Create the observer
IObserver<int> observer = new SimpleObserver<int>();

// 3. Register the observer with the observable
var disposable = observable.Subscribe(observer);

// 4. Publish a value
observable.Publish(1);

// 5. Dispose the observer
disposable.Dispose();
```

*Output*
```
Subscribe on Thread "NonParallelWorker:10"
OnNext(1) thread NonParallelWorker:10
Dispose on Thread NonParallelWorker:10
```

## Schedulers
The core interface we must implement if we want to define a reactive scheduler is IScheduler. The following code shows how to specify the scheduler on which OnNext methods are invoked. We can create our own very simple Scheduler in Figure 4 Custom Scheduler. We can then instruct our code to observe or subscribe on this custom scheduler. 

### Custom Schedulers
```csharp
public class SingleThreadedScheduler : IScheduler
{
  public SingleThreadedScheduler(string threadName)
  {
    Thread t = new Thread(() =>
    {
      while (true)
      {
        try
        {
          Action a = _queue.Take();
          a();
        }
        catch (Exception)
        {
          "SingleThreadedScheduler".Log();
        }
      }
    })
    { Name = threadName, IsBackground = false };
   t.Start();
 }

 public IDisposable Schedule<TState>(TState state, Func<IScheduler, TState, IDisposable> action)
 {
   var d = new SingleAssignmentDisposable();

   _queue.TryAdd(() =>
   {
     if (d.IsDisposed)
       return;

     d.Disposable = action(this, state);
   });

   return d;
  }

  public IDisposable Schedule<TState>(TState state, TimeSpan dueTime, Func<IScheduler, TState, IDisposable> action)
            => Disposable.Empty;

 public IDisposable Schedule<TState>(TState state, DateTimeOffset dueTime,
            Func<IScheduler, TState, IDisposable> action) => Disposable.Empty;


 public DateTimeOffset Now { get; }

 private readonly BlockingCollection<Action> _queue
            = new BlockingCollection<Action>(new ConcurrentQueue<Action>());
}
```
### Explicit Subscription
*C# Sample Code*
```csharp
// 1. Log out the calling thread
"ExplicitMultiThreadedSubsciption".Log();
Console.WriteLine("Current Thread {0}", Thread.CurrentThread.ManagedThreadId);

// 2. Create a scheduler with its own thread 
IScheduler scheduler = new SingleThreadedScheduler("KennyScheduler");

// 3. Create the observable
var observable = new SimpleObservable<int>();

// 4. Create the observer
IObserver<int> observer = new SimpleObserver<int>();

// 5. Register the observer with the observable
var disposable = observable.SubscribeOn(scheduler).Subscribe(observer);

// Make sure the publish does not happen before the subscription as 
// subscription is running on a separate thread
Thread.Sleep(100);

// 6. Publish a value
observable.Publish(1);

// 7. Dispose the observer
disposable.Dispose();
```
*Output*
```
ExplicitMultiThreadedSubsciption - Thread NonParallelWorker 12
Current Thread 12
Subscribe on Thread "KennyScheduler:15"
OnNext(1) thread NonParallelWorker:12
```


## Explicit Observation
*C# Sample Code*
```csharp
// 1. Log out the calling thread
"ExplicitMultiThreadedObservation".Log();

// 2. Create a scheduler with its own thread and a 
//    wait handle to prevent premature completion
IScheduler scheduler = new SingleThreadedScheduler("KennysScheduler");
AutoResetEvent handle = new AutoResetEvent(false);

// 3. Create observable tell it we want to observer
//    on our explicit scheduler
var observable = new SimpleObservable<int>();
var disposable = observable
	.ObserveOn(scheduler)
	.Subscribe(i => i.ToString().Log(), () => handle.Set());

// 4. Publish 2 messages and then complete 
observable.Publish(1);
observable.Publish(2);
observable.Complete();

handle.WaitOne();
disposable.Dispose();
```
*Output*
```
ExplicitMultiThreadedObservation - Thread NonParallelWorker 12
Subscribe on Thread "NonParallelWorker:12"
1 - Thread KennysScheduler 13
2 - Thread KennysScheduler 13
Dispose on Thread NonParallelWorker:12
```