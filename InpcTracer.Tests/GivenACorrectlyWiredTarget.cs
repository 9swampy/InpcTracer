namespace InpcTracer.Tests
{
  using System;
  using FluentAssertions;
  using InpcTracer.Configuration;
  using InpcTracer.Framework;
  using InpcTracer.Tracing;
  using Microsoft.VisualStudio.TestTools.UnitTesting;
  
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
      Action a = () => tracer.PropertyChanged(() => target.PropertyA).MustHaveBeen(Notified.Exactly.Once);
      a.ShouldThrow<InpcTracer.Framework.ExpectationException>();
    }

    [TestMethod]
    public void WhenPropertyChangedThenHasRecordedEventShouldNotRaiseAnError()
    {
      target.PropertyA = true;
      tracer.PropertyChanged(() => target.PropertyA).MustHaveOccurred();
      tracer.PropertyChanged(() => target.PropertyA).MustHaveBeen(Notified.Exactly.Once);
      Action a = () => tracer.PropertyChanged(() => target.PropertyA);
      a.ShouldNotThrow();
      tracer.PropertyChanged(() => target.PropertyA).ExactlyOnce().Should().Be(true);
      Action b = () => tracer.PropertyChanged(() => target.PropertyA).MustHaveBeen(Notified.Like(o => o == 1));
      b.ShouldNotThrow();
    }

    [TestMethod]
    public void WhenPropertyChangedThenFirstRecordedEventShouldMatch()
    {
      target.PropertyB = true;
      Action a = () => tracer.FirstPropertyChanged(() => target.PropertyA);
      a.ShouldThrow<ArgumentException>();
    }

    [TestMethod]
    public void WhenPropertyChangedThenFirstRecordedEventShouldNotThrow()
    {
      target.PropertyB = true;
      Action a = () => tracer.FirstPropertyChanged(() => target.PropertyB);
      a.ShouldNotThrow();
    }

    [TestMethod]
    public void WhenSinglePropertyChangedThenSubsequentThenRecordedEventShouldThrow()
    {
      target.PropertyA = true;
      Action a = () => tracer.FirstPropertyChanged(() => target.PropertyA);
      a.ShouldNotThrow();
      Action b = () => tracer.FirstPropertyChanged(() => target.PropertyA).ThenPropertyChanged(() => target.PropertyB);
      b.ShouldThrow<ArgumentException>();
    }

    [TestMethod]
    public void WhenDoublePropertiesChangedThenSubsequentThenRecordedEventShouldNotThrow()
    {
      target.PropertyA = true;
      target.PropertyB = true;
      Action a = () => tracer.FirstPropertyChanged(() => target.PropertyA);
      a.ShouldNotThrow();
      Action b = () => tracer.FirstPropertyChanged(() => target.PropertyA).ThenPropertyChanged(() => target.PropertyB);
      b.ShouldNotThrow();
    }

    [TestMethod]
    public void WhenSinglePropertyChangedThenMustHaveHappendedTwiceShouldThrow()
    {
      target.PropertyA = true;
      target.PropertyB = true;
      Action a = () => tracer.FirstPropertyChanged(() => target.PropertyA);
      a.ShouldNotThrow();
      Action b = () => tracer.FirstPropertyChanged(() => target.PropertyA).ThenPropertyChanged(() => target.PropertyB);
      b.ShouldNotThrow();
    }

    [TestMethod]
    public void WhenUnmatchedPropertiesChangedThenMustHaveHappendedTwiceShouldThrow()
    {
      target.PropertyA = true;
      target.PropertyB = true;

      Action a = () => tracer.PropertyChanged(() => target.PropertyA).MustHaveBeen(Notified.AtLeast.Twice);
      a.ShouldThrow<ExpectationException>();
      Action b = () => tracer.PropertyChanged(() => target.PropertyA).MustHaveBeen(Notified.Exactly.Twice);
      b.ShouldThrow<ExpectationException>();
    }

    [TestMethod]
    public void WhenPropertyChangedThenHasNotRecordedEventShouldThrow()
    {
      target.PropertyA = true;
      Action a = () => tracer.PropertyChanged(() => target.PropertyA).MustHaveBeen(Notified.Never);
      a.ShouldThrow<ExpectationException>();
    }

    [TestMethod]
    public void WhenPropertyChangedThenHasNotRecordedEventShouldValidate()
    {
      target.PropertyA = true;
      Action a = () => tracer.PropertyChanged(() => target.PropertyA).MustHaveBeen(Notified.Never);
      a.ShouldThrow<ExpectationException>();
    }

    [TestMethod]
    public void WhenCorrectlyWiredPropertyChangedThenHasNotRecordedEventShouldThrow()
    {
      target.PropertyB = true;

      Action a = () => tracer.PropertyChanged(() => target.PropertyA).MustHaveBeen(Notified.Never);
      a.ShouldNotThrow();

      Action b = () => tracer.PropertyChanged(() => target.PropertyB).MustHaveBeen(Notified.Never);
      b.ShouldThrow<ExpectationException>();

      tracer.PropertyChanged(() => target.PropertyB).AtLeastOnce().Should().Be(true);
    }
  }
}
