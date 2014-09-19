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
    private INotifyPropertyChanged target;

    private InpcTracer.EventMonitor<INotifyPropertyChanged> eventMonitor;

    [TestInitialize]
    public void TestInitialise()
    {
      Fake.InitializeFixture(this);

      this.eventMonitor = new EventMonitor<INotifyPropertyChanged>(this.target);
    }

    [TestMethod]
    public void WhenEventRaisedOnceThenItShouldBeRecordedOnce()
    {
      this.target.PropertyChanged += Raise.With<PropertyChangedEventArgs>(new PropertyChangedEventArgs("Hello")).Now;
      this.eventMonitor.Event("PropertyChanged").MustHaveBeen(Notified.Exactly.Once);
    }

    [TestMethod]
    public void WhenEventRaisedTwiceThenItShouldBeRecordedTwice()
    {
      this.target.PropertyChanged += Raise.With<PropertyChangedEventArgs>(new PropertyChangedEventArgs("Hello")).Now;
      this.target.PropertyChanged += Raise.With<PropertyChangedEventArgs>(new PropertyChangedEventArgs("Hello")).Now;
      this.eventMonitor.Event("PropertyChanged").MustHaveBeen(Notified.Exactly.Twice);
    }
  }
}