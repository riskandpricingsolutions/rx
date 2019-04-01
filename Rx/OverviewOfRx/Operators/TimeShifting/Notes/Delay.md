# 

Delay

### Delay(TimeSpan timeSpan)
Simply delays a sequence by a specified TimeSpan. The times between elements remain the same.

*C# Sample Code*
```csharp
DateTime now = DateTime.Now;
EventWaitHandle latch = new AutoResetEvent(false);

var source = Observable.Interval(TimeSpan.FromSeconds(0.5)).Take(4);
var delays = source.Delay(TimeSpan.FromSeconds(1.0));

source.Subscribe(l => WriteLine($"Original {l}   {(DateTime.Now - now).TotalSeconds}"));
delays.Subscribe(l => WriteLine($"Delayed {l}   {(DateTime.Now - now).TotalSeconds}"),()=>latch.Set());

latch.WaitOne();
```
*Output*

```
Original 0   0.5671419
Original 1   1.0641751
Original 2   1.5644976
Delayed 0   1.6014948
Original 3   2.0791571
Delayed 1   2.0985018
Delayed 2   2.6119969
Delayed 3   3.1114934
```

*Marble Diagram*

![Delay(Time Span)](Resources/Delay(TimeSpan).png)




