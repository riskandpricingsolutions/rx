# Transforming Streams
### SelectMany
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