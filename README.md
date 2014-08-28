<h1>InpcTracer</h1>
==========

<b>Inspired by https://gist.github.com/Seikilos/6224204</b><br>
<b>Published at https://www.nuget.org/packages/InpcTracer/</b>
<p>
I needed to clean up testing INotifyPropertyChanged implementation of an application I'm working on. Seikilos' Gist 
inspired me, kudos. I use FakeItEasy and have altered the usage quite a bit to be more akin to it's way of working.
</p>
<p>
I'm only dabbling with Git having been an SVN user for many years. Hopefully I'm not stepping on any toes or doing 
anything incorrect!
</p>
<h5>
<b>Check for one event</b>
</h5>
<p>
Assert.IsTrue(tracer.WhileProcessing(() => p.Active = true).HasRecordedEvent(() => p.Active).ExactlyOnce());
</p>
<h5>
Check for exact order of two events
</h5>
<p>
tracer.WhileProcessing(() => p.Path = "test").FirstRecordedEvent(() => p.Path).ThenRecordedEvent(() => p.Active);
</p>
