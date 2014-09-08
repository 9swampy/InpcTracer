namespace InpcTracer.Tests
{
  using System.ComponentModel;
  using FakeItEasy;
  using InpcTracer.Configuration;
  using Microsoft.VisualStudio.TestTools.UnitTesting;

  [TestClass]
  public class GivenMonitoringEventMonitor
  {
    [Fake]
    internal INotifyPropertyChanged Target;

    private InpcTracer.Tracing.EventMonitor<INotifyPropertyChanged> eventMonitor;

    [TestInitialize]
    public void TestInitialise()
    {
      Fake.InitializeFixture(this);

      this.eventMonitor = new Tracing.EventMonitor<INotifyPropertyChanged>(this.Target);
    }

    [TestMethod]
    public void WhenEventRaisedOnceThenItShouldBeRecordedOnce()
    {
      this.Target.PropertyChanged += Raise.With<PropertyChangedEventArgs>(new PropertyChangedEventArgs("Hello")).Now;
      this.eventMonitor.Event("PropertyChanged").MustHaveBeen(Notified.Exactly.Once);
    }

    [TestMethod]
    public void WhenEventRaisedTwiceThenItShouldBeRecordedTwice()
    {
      this.Target.PropertyChanged += Raise.With<PropertyChangedEventArgs>(new PropertyChangedEventArgs("Hello")).Now;
      this.Target.PropertyChanged += Raise.With<PropertyChangedEventArgs>(new PropertyChangedEventArgs("Hello")).Now;
      this.eventMonitor.Event("PropertyChanged").MustHaveBeen(Notified.Exactly.Twice);
    }
  }
}