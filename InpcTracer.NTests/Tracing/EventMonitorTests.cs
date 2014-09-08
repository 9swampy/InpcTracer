namespace InpcTracer.NTests.Tracing
{
  using System;
  using System.ComponentModel;
  using FakeItEasy;
  using FluentAssertions;
  using InpcTracer.Configuration;
  using InpcTracer.Tracing;
  using NUnit.Framework;

  [TestFixture]
  public class EventMonitorTests
  {
    [Fake]
    public INotifyPropertyChanged Target;
    
    private EventMonitor<INotifyPropertyChanged> eventMonitor;

    [SetUp]
    public void Setup()
    {
      Fake.InitializeFixture(this);

      this.eventMonitor = new EventMonitor<INotifyPropertyChanged>(this.Target);
    }

    [Test]
    public void AfterResetRecordedEventShouldBeCleared()
    {
      this.Target.PropertyChanged += Raise.With<PropertyChangedEventArgs>(new PropertyChangedEventArgs("")).Now;
      this.eventMonitor.Event("PropertyChanged").MustHaveBeen(Notified.Exactly.Once);
      this.eventMonitor.Reset();
      this.eventMonitor.Event("PropertyChanged").MustHaveBeen(Notified.Never);
    }

    [Test]
    public void WhenArgumentNotEventThenShouldThrowExplainingRequirement()
    {
      string expectedMessage = "The parameter must specify an event.";

      Action act = () => this.eventMonitor.Event("NotAnEvent");

      act.ShouldThrow<ArgumentException>().WithMessage(expectedMessage);
    }
  }
}