<html>
  <h1>InpcTracer</h1>
  ==========

  <b>
    Inspired by <a href="https://gist.github.com/Seikilos/6224204">Seikilos</a>
  </b><br />
  <b>
    Published on <a href="https://www.nuget.org/packages/InpcTracer/">NuGet</a>
  </b>
  <p>
    I needed to clean up testing of INotifyPropertyChanged implementations in an application I'm working on. Seikilos' Gist
    inspired me, kudos. I use FakeItEasy and have altered the usage quite a bit by adopting their MustHave(Repeated) fluent
    syntax to be more consistent.
  </p>
  <p>
    I'm only dabbling with Git having been an SVN user for many years. Hopefully I'm not stepping on any toes or doing
    anything incorrect!
  </p>
  <h5>
    <b>Arrange</b>
  </h5>
```csharp
ITraceable target = A.Fake<ITraceable>();
A.CallTo(target).Where(x => x.Method.Name == "set_Active")
.Invokes(() =>
    {
      target.PropertyChanged += 
        Raise.With(new PropertyChangedEventArgs("Active")).Now;
    });
    
var tracer = new InpcTracer.InpcTracer<ITraceable>(target);
```

  <h5>
    <b>Check for one event</b>
  </h5>
```csharp
Assert.IsTrue(tracer.WhileProcessing(() => target.Active = true)
                    .RecordedEvent(() => target.Active)
                    .ExactlyOnce());
// or
tracer.WhileProcessing(() => target.Active = true)
      .RecordedEvent(() => target.Active)
      .MustHaveHappened(Repeated.Exactly.Once);
```
  <h5>
    Check for exact order of two events
  </h5>
```csharp
// Check for exact order of two events
tracer.WhileProcessing(() =>
      {
        target.Active = false;
        target.Active = true;
      })
      .FirstRecordedEvent(() => target.Active)
      .ThenRecordedEvent(() => target.Active);
```
</html>
