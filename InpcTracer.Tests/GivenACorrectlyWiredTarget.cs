namespace InpcTracer.Tests
{
  using InpcTracer.Configuration;
  using InpcTracer.Framework;
  using InpcTracer.Tracing;
  using Microsoft.VisualStudio.TestTools.UnitTesting;
  using Shouldly;
  using System;

  [TestClass]
  public class GivenACorrectlyWiredTarget
  {
    private readonly static IExampleNotifyPropertyChanged target;
    private static InpcTracer.InpcTracer<IExampleNotifyPropertyChanged> tracer;

    static GivenACorrectlyWiredTarget()
    {
      target = ExampleTargetProvider.ACorrectlyWiredTarget();
    }

    [TestInitialize]
    public void TestInitialize()
    {
      tracer = new InpcTracer.InpcTracer<IExampleNotifyPropertyChanged>(target, new ExpressionValidator());
    }

    [TestMethod]
    public void WhenNoPropertyChangedThenHasRecordedEventShouldRaiseAnError()
    {
      Should.Throw<InpcTracer.Framework.ExpectationException>(() => tracer.RecordedEvent(() => target.PropertyA).MustHaveHappened(Repeated.Exactly.Once));
    }

    [TestMethod]
    public void WhenPropertyChangedThenHasRecordedEventShouldNotRaiseAnError()
    {
      target.PropertyA = true;
      tracer.RecordedEvent(() => target.PropertyA).MustHaveHappened();
      tracer.RecordedEvent(() => target.PropertyA).MustHaveHappened(Repeated.Exactly.Once);
      Should.NotThrow(() => tracer.RecordedEvent(() => target.PropertyA));
      tracer.RecordedEvent(() => target.PropertyA).ExactlyOnce().ShouldBe(true);
      Should.NotThrow(() => tracer.RecordedEvent(() => target.PropertyA).MustHaveHappened(Repeated.Like(o => o == 1)));
    }

    [TestMethod]
    public void WhenPropertyChangedThenFirstRecordedEventShouldMatch()
    {
      target.PropertyB = true;
      Should.Throw<ArgumentException>(() => tracer.FirstRecordedEvent(() => target.PropertyA));
    }

    [TestMethod]
    public void WhenPropertyChangedThenFirstRecordedEventShouldNotThrow()
    {
      target.PropertyB = true;
      Should.NotThrow(() => tracer.FirstRecordedEvent(() => target.PropertyB));
    }

    [TestMethod]
    public void WhenSinglePropertyChangedThenSubsequentThenRecordedEventShouldThrow()
    {
      target.PropertyA = true;
      Should.NotThrow(() => tracer.FirstRecordedEvent(() => target.PropertyA));
      Should.Throw<ArgumentException>(() => tracer.FirstRecordedEvent(() => target.PropertyA).ThenRecordedEvent(() => target.PropertyB));
    }

    [TestMethod]
    public void WhenDoublePropertiesChangedThenSubsequentThenRecordedEventShouldNotThrow()
    {
      target.PropertyA = true;
      target.PropertyB = true;
      Should.NotThrow(() => tracer.FirstRecordedEvent(() => target.PropertyA));
      Should.NotThrow(() => tracer.FirstRecordedEvent(() => target.PropertyA).ThenRecordedEvent(() => target.PropertyB));
    }

    [TestMethod]
    public void WhenSinglePropertyChangedThenMustHaveHappendedTwiceShouldThrow()
    {
      target.PropertyA = true;
      target.PropertyB = true;
      Should.NotThrow(() => tracer.FirstRecordedEvent(() => target.PropertyA));
      Should.NotThrow(() => tracer.FirstRecordedEvent(() => target.PropertyA).ThenRecordedEvent(() => target.PropertyB));
    }

    [TestMethod]
    public void WhenUnmatchedPropertiesChangedThenMustHaveHappendedTwiceShouldThrow()
    {
      target.PropertyA = true;
      target.PropertyB = true;
      Should.Throw<ExpectationException>(() => tracer.RecordedEvent(() => target.PropertyA).MustHaveHappened(Repeated.AtLeast.Twice));
      Should.Throw<ExpectationException>(() => tracer.RecordedEvent(() => target.PropertyA).MustHaveHappened(Repeated.Exactly.Twice));
    }

    [TestMethod]
    public void WhenPropertyChangedThenHasNotRecordedEventShouldThrow()
    {
      target.PropertyA = true;
      Should.Throw<ArgumentException>(() => tracer.HasNotRecordedEvent(() => target.PropertyA));
    }

    [TestMethod]
    public void WhenPropertyChangedThenHasNotRecordedEventShouldValidate()
    {
      target.PropertyA = true;
      Should.Throw<ArgumentException>(() => tracer.HasNotRecordedEvent(() => target.PropertyA));
    }
  }
}
