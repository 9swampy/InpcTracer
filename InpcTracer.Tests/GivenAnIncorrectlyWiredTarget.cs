namespace InpcTracer.Tests
{
  using FakeItEasy;
  using InpcTracer.Tracing;
  using Microsoft.VisualStudio.TestTools.UnitTesting;
  using Shouldly;
  using System;
  using System.ComponentModel;
  using Repeated = InpcTracer.Configuration.Repeated;

  [TestClass]
  public class GivenAnIncorrectlyWiredTarget
  {
    private readonly static IExampleNotifyPropertyChanged target;
    private static InpcTracer.InpcTracer<IExampleNotifyPropertyChanged> tracer;

    static GivenAnIncorrectlyWiredTarget()
    {
      target = FakeItEasy.A.Fake<IExampleNotifyPropertyChanged>();
      FakeItEasy.A.CallTo(target).Where(x => x.Method.Name == "set_PropertyA")
                .Invokes(() =>
                {
                  target.PropertyChanged += Raise.With(new PropertyChangedEventArgs("PropertyA")).Now;
                });
    }

    [TestInitialize]
    public void TestInitialize()
    {
      tracer = new InpcTracer.InpcTracer<IExampleNotifyPropertyChanged>(target, new ExpressionValidator());
    }

    [TestMethod]
    public void WhenNoPropertyChangedThenHasRecordedEventShouldRaiseAnError()
    {
      Should.Throw<InpcTracer.Framework.ExpectationException>(() => tracer.RecordedEvent(() => target.PropertyA).MustHaveHappened());
    }

    [TestMethod]
    public void WhenIncorrectlyWiredPropertyChangedThenHasRecordedEventShouldRaiseAnError()
    {
      target.PropertyB = true;
      Should.Throw<InpcTracer.Framework.ExpectationException>(() => tracer.RecordedEvent(() => target.PropertyB).MustHaveHappened());
    }

    [TestMethod]
    public void WhenOtherPropertyChangedThenFirstRecordedEventShouldThrow()
    {
      target.PropertyA = true;
      Should.Throw<ArgumentException>(() => tracer.FirstRecordedEvent(() => target.PropertyB));
    }

    [TestMethod]
    public void WhenIncorrectlyWiredPropertyChangedThenFirstRecordedEventShouldThrow()
    {
      target.PropertyB = true;
      Should.Throw<ArgumentException>(() => tracer.FirstRecordedEvent(() => target.PropertyB));
    }

    [TestMethod]
    public void WhenSinglePropertyChangedThenSubsequentThenRecordedEventShouldThrow()
    {
      target.PropertyA = true;
      Should.NotThrow(() => tracer.FirstRecordedEvent(() => target.PropertyA));
      Should.Throw<ArgumentException>(() => tracer.FirstRecordedEvent(() => target.PropertyA).ThenRecordedEvent(() => target.PropertyB));
    }

    [TestMethod]
    public void WhenDoublePropertiesChangedThenSubsequentThenRecordedEventShouldThrowAsIncorrectlyWired()
    {
      target.PropertyA = true;
      target.PropertyB = true;
      Should.NotThrow(() => tracer.FirstRecordedEvent(() => target.PropertyA));
      Should.Throw<ArgumentException>(() => tracer.FirstRecordedEvent(() => target.PropertyA).ThenRecordedEvent(() => target.PropertyB));
    }

    [TestMethod]
    public void WhenIncorrectlyWiredPropertyChangedThenHasNotRecordedEventShouldNotThrow()
    {
      target.PropertyB = true;
      Should.NotThrow(() => tracer.RecordedEvent(() => target.PropertyB).MustHaveHappened(Repeated.Never));
      Should.NotThrow(() => tracer.RecordedEvent(() => target.PropertyB).MustHaveHappened(InpcTracer.Configuration.Repeated.Never));
      tracer.RecordedEvent(() => target.PropertyB).AtLeastOnce().ShouldBe(false);
    }
  }
}
