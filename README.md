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
  <p>
    ITraceable target = A.Fake&lt;ITraceable&gt;();<br/>
    A.CallTo(target).Where(x => x.Method.Name == "set_Active")<br/>
    .Invokes(() =><br/>
    {<br/>
    target.PropertyChanged += Raise.With(new PropertyChangedEventArgs("Active")).Now;<br/>
    });<br/>
    <br/>
    var tracer = new InpcTracer.InpcTracer&lt;ITraceable&gt;(target);
  </p>
  <h5>
    <b>Check for one event</b>
  </h5>
  <p>
    Assert.IsTrue(tracer.WhileProcessing(() =&gt; target.Active = true).RecordedEvent(() => target.Active).ExactlyOnce());<br/>
    // or<br/>
    tracer.WhileProcessing(() => target.Active = true).RecordedEvent(() => target.Active).MustHaveHappened(Repeated.Exactly.Once);
  </p>
  <h5>
    Check for exact order of two events
  </h5>
  <p>
    // Check for exact order of two events<br/>
    tracer.WhileProcessing(() =&gt;<br/>
    {<br/>
    target.Active = false;<br/>
    target.Active = true;<br/>
    }).FirstRecordedEvent(() =&gt; target.Active).ThenRecordedEvent(() =&gt; target.Active);
  </p>
</html>
