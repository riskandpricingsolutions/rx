# Rx Operator Cheet Sheet

## Observable.Return(T)
Returns an observable consisting of single value and then calls OnCompleted
##### C#
```csharp
Observable.Return(1)
 .Subscribe(WriteLine, () => WriteLine("OnCompleted"));
```
##### Marble
![Return(T)](Resources/Return(T).png)

## Observable.Range(int,int)
Returns an observable over a sequence of consecutive integers
##### C#
```csharp
Observable.Range(1,3)
 .Subscribe(WriteLine, () => WriteLine("OnCompleted"));
```
##### Marble
![Range(Int,Int)](Resources/Range(int,int).png)

## Observable.Repeat(TResult,int)
Repeat the give value a specified number of times

##### C#

```csharp
Observable.Repeat(2,3)
  .Subscribe(WriteLine, () => WriteLine("OnCompleted\n"));
```
##### Marble
![Repeat(T,Int)](Resources/Repeat(T,int).png)

## Observable.Repeat(IObservable&lt;TResult&gt;,int)
Repeat the give sequence the specified number of times

##### C#

```csharp
Observable.Range(0, 2)
 .Repeat(2)
 .Subscribe(WriteLine, () => WriteLine("OnCompleted\n"));
```
##### Marble
![Repeat(T Result,Int)](Resources/Repeat(TResult,int).png)

## Observable.Generate
Supports the creation of more complex sequences of event
##### C#
```csharp
AutoResetEvent h = new AutoResetEvent(false);

WriteLine("Generate");
Observable
	.Generate(
		0,
		i => i < 3,
		i => ++i,
		i => $"Value {i}",
		i => TimeSpan.FromSeconds(i))
	.SubscribeOn(DefaultScheduler.Instance)
    .ObserveOn(DefaultScheduler.Instance)
    .Subscribe(WriteLine, () => h.Set());

h.WaitOne();
```
##### Marble

## Scan(TAccumulate,Func<TAccumulate, TSource, TAccumulate>)
An accumulator which produces a sequence of accumulated values
##### C#
```csharp
Observable.Range(1, 3)
 .Scan(0,(cum, i1) => cum+i1)
 .Subscribe(WriteLine, () => WriteLine("OnCompleted\n"));
```
##### Marble
![Scan](Resources/Scan.png)

## SelectMany
SelectMany seems to work by taking an observable sequence and using each value from that sequence to generate another sequence. The generated sequences are then merged (flattened) together
##### C#
```csharp
Subject<int>[] subs = new Subject<int>[]
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
```
##### Marble
![Select Many](Resources/SelectMany.png)


## Buffer(int)
Consider the following code which highlights the use of **Buffer(int)** to transform an observable sequence of int into an observable sequence of list of int. We consider each List<int> a buffer. 

##### C#
```csharp
SimpleObservable<int> myObservable = new SimpleObservable<int>();
IObservable<IList<int>> buffered = myObservable.Buffer(2);

buffered.Subscribe(ints => Console.WriteLine(string.Join(",", ints)));

myObservable.Publish(1);
myObservable.Publish(2);
myObservable.Publish(3);
myObservable.Publish(4);
myObservable.Publish(5);
myObservable.Complete();
```
The following figure shows graphically what is happening. The buffers contain at most Count items. Notice the final buffer has less than two items because we invoke Complete on the source observable. 
##### Marble Diagram
![Buffer(Int)](Resources/Buffer(int).png)

## Buffer(Func<IObservable<TClosingSelector>> ClosingSelector)
Whenever we publish a value from the closing selector observable the buffer will take whatever it has and flush it out. It does not really seem to matter what the value or type of the buffer close event is

##### C#
```csharp
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
```

##### Marble Diagram
![Buffer(Func Observable)](Resources/Buffer(FuncObservable).png)

## Buffer(int count,int skip)

```csharp           
SimpleObservable<int> sourceObs = new SimpleObservable<int>();

// Buffers will be started on the 1st, 4th, 7th element. Each buffer 
// will have two elements. 
IObservable<IList<int>> buffer = sourceObs.Buffer(2, 3);
buffer.Subscribe( ints => Console.WriteLine(string.Join(",",ints)));

sourceObs.Publish(1);
sourceObs.Publish(2);
sourceObs.Publish(3);
sourceObs.Publish(4);
sourceObs.Publish(5);
sourceObs.Publish(6);
```
![Buffer(Int,Int)](Resources/Buffer(int,int).png)
