namespace InpcTracer.NTests
{
  using System.ComponentModel;
  using FakeItEasy;
  using NUnit.Framework;
  
  [TestFixture]
  public class GivenMonitoringEventMonitor
  {
    [Fake]
    private INotifyPropertyChanged target;

    private InpcTracer.EventMonitor<INotifyPropertyChanged> eventMonitor;

    [SetUp]
    public void TestInitialise()
    {
      Fake.InitializeFixture(this);

      this.eventMonitor = new EventMonitor<INotifyPropertyChanged>(this.target);
    }

    [Test]
    public void WhenEventRaisedOnceThenItShouldBeRecordedOnce()
    {
      this.target.PropertyChanged += Raise.With<PropertyChangedEventArgs>(new PropertyChangedEventArgs("Hello")).Now;
      this.eventMonitor.Event("PropertyChanged").MustHaveBeen(Raised.Exactly.Once);
    }

    [Test]
    public void WhenEventRaisedTwiceThenItShouldBeRecordedTwice()
    {
      this.target.PropertyChanged += Raise.With<PropertyChangedEventArgs>(new PropertyChangedEventArgs("Hello")).Now;
      this.target.PropertyChanged += Raise.With<PropertyChangedEventArgs>(new PropertyChangedEventArgs("Hello")).Now;
      this.eventMonitor.Event("PropertyChanged").MustHaveBeen(Raised.Exactly.Twice);
    }
  }
}