# Time Shifting CheatSheet
### Buffer(int count)
Consider the following code which highlights the use of **Buffer(int)** to transform an observable sequence of int into an observable sequence of list of int. We consider each List<int> a buffer. Most of the examples below define the buffers using element counts however we can equally well use time intervals. The API support both approaches

##### C#
```csharp
Observable
 .Range(0, 5)
 .Buffer(2)
 .Subscribe(ints => WriteLine(string.Join(",", ints)));
```
The following figure shows graphically what is happening. The buffers contain at most count items. Notice the final buffer has less than two items because the source stream completes
##### Marble Diagram
![Buffer(Int)](Resources/Buffer(int).png)


## Buffer(int count,int skip) - Skipping Elements
We start a new buffer every skip elements and each buffer is at most count elements long. If skip is greater than count as in the example below then we miss out some elements 

##### C#
```csharp           
Observable.Range(1, 6)
 .Buffer(2, 3)
 .Subscribe(ints => WriteLine(string.Join(",", ints)));
```
##### Marble Diagram
![Buffer(Int,Int)](Resources/Buffer(int,int).png)

## Buffer(int count,int skip) - Overlapping Elements
We start a new buffer every skip elements and each buffer is at most count elements long. If skip is less than count as in the example below then we overlap out some elements 

##### C#
```csharp           
Observable.Range(1, 5)
 .Buffer(3, 2)
 .Subscribe(ints => WriteLine(string.Join(",", ints)));
```
##### Marble Diagram
![Buffer(Int,Int)Overlap](Resources/Buffer(int,int)Overlap.png)


### Buffer(Func<IObservable<TClosingSelector>> ClosingSelector)
Whenever we publish a value from the closing selector observable the buffer will take whatever it has and flush it out. It does not really seem to matter what the value or type of the buffer close event is

##### C#
```csharp
EventWaitHandle latch = new AutoResetEvent(false);

var obs = Observable
 .Interval(TimeSpan.FromSeconds(0.3))
 .Take(10);

var closing = Observable
 .Interval(TimeSpan.FromSeconds(1.0))
 .Take(2);

 obs
  .Buffer(closing)
  .Subscribe(ints => WriteLine(string.Join(",", ints)),()=>latch.Set());

latch.WaitOne();
```
##### Marble Diagram
![Buffer(Func Observable)](Resources/Buffer(FuncObservable).png)

### Buffer(IObservable<TBufferOpening> bufferOpenings, Func<TBufferOpening, IObservable<TBufferClosing>> bufferClosingSelector)
This form of the function uses sequences to determin the buffer openings and closings
##### C#
```csharp
EventWaitHandle latch = new AutoResetEvent(false);

var obs = Observable
 .Interval(TimeSpan.FromSeconds(0.3))
 .Take(10);

var opening = Observable
 .Interval(TimeSpan.FromSeconds(0.7))
 Take(2);

var closing = Observable
 .Timer(TimeSpan.FromSeconds(0.5))
 .Take(2);

obs
 .Buffer(opening, i => closing)
 .Subscribe(ints => WriteLine($"({string.Join(",", ints)})"), () => latch.Set());

latch.WaitOne();
```
##### Marble Diagram
![B Uffer Open Close](Resources/BUfferOpenClose.png)







