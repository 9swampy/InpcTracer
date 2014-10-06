namespace InpcTracer.NTests
{
  using System;
  using System.ComponentModel;
  using System.Threading.Tasks;
  using FakeItEasy;
  using FluentAssertions;
  using NUnit.Framework;

  [TestFixture]
  public class GivenEventMonitorAndAsyncEvent
  {
    private const int timeout = 500;
    private static IExampleNotifyPropertyChanged target;
    private EventMonitor<IExampleNotifyPropertyChanged> eventMonitor;

    [TestFixtureSetUp]
    public void ClassInitialize()
    {
      target = A.Fake<IExampleNotifyPropertyChanged>();
      FakeItEasy.A.CallTo(target).Where(x => x.Method.Name == "set_PropertyA")
                .Invokes(async () =>
                {
                  await Task.Delay(50);
                  target.PropertyChanged += Raise.With(new PropertyChangedEventArgs("PropertyA")).Now;
                });
    }

    [SetUp]
    public void TestInitialize()
    {
      this.eventMonitor = new EventMonitor<IExampleNotifyPropertyChanged>(target);
    }

    [Test]
    public async Task WhenEventRaisedOnceWithinTestTimeoutThenEventShouldBeMonitoredExactlyOnce()
    { 
      target.PropertyA = "set";
      await Task.Delay(timeout);
      eventMonitor.Event("PropertyChanged").MustHaveBeen(Raised.Exactly.Once);
    }

    [Test]
    public void WhenEventRaisedOnceWithinTimeoutThenEventShouldBeMonitoredExactlyOnce()
    {
      target.PropertyA = "set";
      eventMonitor.Event("PropertyChanged", timeout).MustHaveBeen(Raised.Exactly.Once);
    }

    [Test]
    public async Task WhenEventRaisedTwiceWithinTestTimeoutThenEventShouldBeMonitoredExactlyTwice()
    {
      target.PropertyA = "set";
      target.PropertyA = "set";
      await Task.Delay(timeout);
      eventMonitor.Event("PropertyChanged").MustHaveBeen(Raised.Exactly.Twice);
    }

    [Test]
    public void WhenEventRaisedTwiceWithinTimeoutThenEventShouldBeMonitoredExactlyTwice()
    {
      target.PropertyA = "set";
      target.PropertyA = "set";
      eventMonitor.Event("PropertyChanged", timeout).MustHaveBeen(Raised.Exactly.Twice);
    }

    [Test]
    public async Task WhenEventRaisedTwiceWithinTestTimeoutThenEventShouldNotBeMonitoredExactlyOnce()
    {
      target.PropertyA = "set";
      target.PropertyA = "set";
      await Task.Delay(timeout);
      
      Action act = () => eventMonitor.Event("PropertyChanged").MustHaveBeen(Raised.Exactly.Once);

      act.ShouldThrow<InpcTracer.Framework.ExpectationException>().And.Message.Should().Contain("PropertyChanged repeated 2 times");
    }

    [Test]
    public void WhenEventRaisedTwiceWithinTimeoutThenEventShouldNotBeMonitoredExactlyOnce()
    {
      target.PropertyA = "set";
      target.PropertyA = "set";

      Action act = () => eventMonitor.Event("PropertyChanged", timeout).MustHaveBeen(Raised.Exactly.Once);

      act.ShouldThrow<InpcTracer.Framework.ExpectationException>().And.Message.Should().Contain("PropertyChanged repeated 2 times");
    }

    [Test]
    public async Task WhenEventRaisedTwiceWithinTestTimeoutThenEventShouldBeMonitoredAtLeastOnce()
    {
      target.PropertyA = "set";
      target.PropertyA = "set";
      await Task.Delay(timeout);

      Action act = () => eventMonitor.Event("PropertyChanged").MustHaveBeen(Raised.AtLeast.Once);

      act.ShouldNotThrow<InpcTracer.Framework.ExpectationException>();
    }

    [Test]
    public void WhenEventRaisedTwiceWithinTimeoutThenEventShouldBeMonitoredAtLeastOnce()
    {
      target.PropertyA = "set";
      target.PropertyA = "set";

      Action act = () => eventMonitor.Event("PropertyChanged", timeout).MustHaveBeen(Raised.AtLeast.Once);

      act.ShouldNotThrow<InpcTracer.Framework.ExpectationException>();
    }
  }
}
