namespace InpcTracer.Tests
{
  using System;
  using System.Collections.Generic;
  using System.ComponentModel;
  using System.Linq;
  using System.Text;
  using System.Threading.Tasks;
  using FakeItEasy;
  using InpcTracer.Configuration;
  using InpcTracer.Tracing;
  using Microsoft.VisualStudio.TestTools.UnitTesting;
  using FluentAssertions;

  [TestClass]
  public class GivenEventMonitorAndAsyncEvent
  {
    private const int timeout = 500;
    private static IExampleNotifyPropertyChanged target;
    private EventMonitor<IExampleNotifyPropertyChanged> eventMonitor;

    [ClassInitialize]
    public static void ClassInitialize(TestContext context)
    {
      target = A.Fake<IExampleNotifyPropertyChanged>();
      FakeItEasy.A.CallTo(target).Where(x => x.Method.Name == "set_PropertyA")
                .Invokes(async () =>
                {
                  await Task.Delay(50);
                  target.PropertyChanged += Raise.With(new PropertyChangedEventArgs("PropertyA")).Now;
                });
    }

    [TestInitialize]
    public void TestInitialize()
    {
      this.eventMonitor = new EventMonitor<IExampleNotifyPropertyChanged>(target);
    }

    [TestMethod]
    public async Task WhenEventRaisedOnceWithinTestTimeoutThenEventShouldBeMonitoredExactlyOnce()
    { 
      target.PropertyA = "set";
      await Task.Delay(timeout);
      eventMonitor.Event("PropertyChanged").MustHaveBeen(Raised.Exactly.Once);
    }

    [TestMethod]
    public void WhenEventRaisedOnceWithinTimeoutThenEventShouldBeMonitoredExactlyOnce()
    {
      target.PropertyA = "set";
      eventMonitor.Event("PropertyChanged", timeout).MustHaveBeen(Raised.Exactly.Once);
    }

    [TestMethod]
    public async Task WhenEventRaisedTwiceWithinTestTimeoutThenEventShouldBeMonitoredExactlyTwice()
    {
      target.PropertyA = "set";
      target.PropertyA = "set";
      await Task.Delay(timeout);
      eventMonitor.Event("PropertyChanged").MustHaveBeen(Raised.Exactly.Twice);
    }

    [TestMethod]
    public void WhenEventRaisedTwiceWithinTimeoutThenEventShouldBeMonitoredExactlyTwice()
    {
      target.PropertyA = "set";
      target.PropertyA = "set";
      eventMonitor.Event("PropertyChanged", timeout).MustHaveBeen(Raised.Exactly.Twice);
    }

    [TestMethod]
    public async Task WhenEventRaisedTwiceWithinTestTimeoutThenEventShouldNotBeMonitoredExactlyOnce()
    {
      target.PropertyA = "set";
      target.PropertyA = "set";
      await Task.Delay(timeout);
      
      Action act = () => eventMonitor.Event("PropertyChanged").MustHaveBeen(Raised.Exactly.Once);

      act.ShouldThrow<InpcTracer.Framework.ExpectationException>().And.Message.Should().Contain("PropertyChanged repeated 2 times");
    }

    [TestMethod]
    public void WhenEventRaisedTwiceWithinTimeoutThenEventShouldNotBeMonitoredExactlyOnce()
    {
      target.PropertyA = "set";
      target.PropertyA = "set";

      Action act = () => eventMonitor.Event("PropertyChanged", timeout).MustHaveBeen(Raised.Exactly.Once);

      act.ShouldThrow<InpcTracer.Framework.ExpectationException>().And.Message.Should().Contain("PropertyChanged repeated 2 times");
    }

    [TestMethod]
    public async Task WhenEventRaisedTwiceWithinTestTimeoutThenEventShouldBeMonitoredAtLeastOnce()
    {
      target.PropertyA = "set";
      target.PropertyA = "set";
      await Task.Delay(timeout);

      Action act = () => eventMonitor.Event("PropertyChanged").MustHaveBeen(Raised.AtLeast.Once);

      act.ShouldNotThrow<InpcTracer.Framework.ExpectationException>();
    }

    [TestMethod]
    public void WhenEventRaisedTwiceWithinTimeoutThenEventShouldBeMonitoredAtLeastOnce()
    {
      target.PropertyA = "set";
      target.PropertyA = "set";

      Action act = () => eventMonitor.Event("PropertyChanged", timeout).MustHaveBeen(Raised.AtLeast.Once);

      act.ShouldNotThrow<InpcTracer.Framework.ExpectationException>();
    }
  }
}
