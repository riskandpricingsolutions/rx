﻿# Rx Operator Cheet Sheet

## Observable.Return(T)
Returns an observable with a single value and then calls OnCompleted
##### C#
```csharp
Observable.Return(1)
 .Subscribe(WriteLine, () => WriteLine("OnCompleted"));
```
##### Marble
![Return(T)](Resources/Return(T).png)


## Observable.Range(int,int)
##### C#
```csharp
Observable.Range(0,3)
 .Subscribe(WriteLine, () => WriteLine("OnCompleted"));
```
##### Marble
![Range(Int,Int)](Resources/Range(int,int).png)

## Observable.Repeat(int,int)
##### C#

```csharp
GenerateObservable(0, 2)
 .Repeat(2)
 .Subscribe(Console.WriteLine);
```
##### Marble
![Repeat(Int)](Resources/Repeat(int).png)


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