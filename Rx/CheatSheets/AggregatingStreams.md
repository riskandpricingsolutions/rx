# Aggregating Streams
### Scan(TAccumulate,Func<TAccumulate, TSource, TAccumulate>)
An accumulator which produces a sequence of accumulated values
##### C#
```csharp
Observable.Range(1, 3)
 .Scan(0,(cum, i1) => cum+i1)
 .Subscribe(WriteLine, () => WriteLine("OnCompleted\n"));
```
##### Marble
![Scan](Resources/Scan.png)
