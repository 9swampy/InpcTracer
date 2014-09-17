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
    private static readonly IExampleNotifyPropertyChanged Target;
    private static InpcTracer.InpcTracer<IExampleNotifyPropertyChanged> tracer;

    static GivenACorrectlyWiredTarget()
    {
      Target = ExampleTargetProvider.ACorrectlyWiredTarget();
    }

    [TestInitialize]
    public void TestInitialize()
    {
      tracer = new InpcTracer.InpcTracer<IExampleNotifyPropertyChanged>(Target, new ExpressionValidator());
    }

    [TestMethod]
    public void WhenNoPropertyChangedThenHasRecordedEventShouldRaiseAnError()
    {
      Action a = () => tracer.PropertyChanged(() => Target.PropertyA).MustHaveBeen(Notified.Exactly.Once);
      a.ShouldThrow<InpcTracer.Framework.ExpectationException>();
    }

    [TestMethod]
    public void WhenPropertyChangedThenHasRecordedEventShouldNotRaiseAnError()
    {
      Target.PropertyA = true;
      tracer.PropertyChanged(() => Target.PropertyA).MustHaveOccurred();
      tracer.PropertyChanged(() => Target.PropertyA).MustHaveBeen(Notified.Exactly.Once);
      Action a = () => tracer.PropertyChanged(() => Target.PropertyA);
      a.ShouldNotThrow();
      tracer.PropertyChanged(() => Target.PropertyA).ExactlyOnce().Should().Be(true);
      Action b = () => tracer.PropertyChanged(() => Target.PropertyA).MustHaveBeen(Notified.Like(o => o == 1 ? RepeatMatch.SatisfiedPending : RepeatMatch.Unsatisfied));
      b.ShouldNotThrow();
    }

    [TestMethod]
    public void WhenPropertyChangedThenFirstRecordedEventShouldMatch()
    {
      Target.PropertyB = true;
      Action a = () => tracer.FirstPropertyChanged(() => Target.PropertyA);
      a.ShouldThrow<ArgumentException>();
    }

    [TestMethod]
    public void WhenPropertyChangedThenFirstRecordedEventShouldNotThrow()
    {
      Target.PropertyB = true;
      Action a = () => tracer.FirstPropertyChanged(() => Target.PropertyB);
      a.ShouldNotThrow();
    }

    [TestMethod]
    public void WhenSinglePropertyChangedThenSubsequentThenRecordedEventShouldThrow()
    {
      Target.PropertyA = true;
      Action a = () => tracer.FirstPropertyChanged(() => Target.PropertyA);
      a.ShouldNotThrow();
      Action b = () => tracer.FirstPropertyChanged(() => Target.PropertyA).ThenPropertyChanged(() => Target.PropertyB);
      b.ShouldThrow<ArgumentException>();
    }

    [TestMethod]
    public void WhenDoublePropertiesChangedThenSubsequentThenRecordedEventShouldNotThrow()
    {
      Target.PropertyA = true;
      Target.PropertyB = true;
      Action a = () => tracer.FirstPropertyChanged(() => Target.PropertyA);
      a.ShouldNotThrow();
      Action b = () => tracer.FirstPropertyChanged(() => Target.PropertyA).ThenPropertyChanged(() => Target.PropertyB);
      b.ShouldNotThrow();
    }

    [TestMethod]
    public void WhenSinglePropertyChangedThenMustHaveHappendedTwiceShouldThrow()
    {
      Target.PropertyA = true;
      Target.PropertyB = true;
      Action a = () => tracer.FirstPropertyChanged(() => Target.PropertyA);
      a.ShouldNotThrow();
      Action b = () => tracer.FirstPropertyChanged(() => Target.PropertyA).ThenPropertyChanged(() => Target.PropertyB);
      b.ShouldNotThrow();
    }

    [TestMethod]
    public void WhenUnmatchedPropertiesChangedThenMustHaveHappendedTwiceShouldThrow()
    {
      Target.PropertyA = true;
      Target.PropertyB = true;

      Action a = () => tracer.PropertyChanged(() => Target.PropertyA).MustHaveBeen(Notified.AtLeast.Twice);
      a.ShouldThrow<ExpectationException>();
      Action b = () => tracer.PropertyChanged(() => Target.PropertyA).MustHaveBeen(Notified.Exactly.Twice);
      b.ShouldThrow<ExpectationException>();
    }

    [TestMethod]
    public void WhenPropertyChangedThenHasNotRecordedEventShouldThrow()
    {
      Target.PropertyA = true;
      Action a = () => tracer.PropertyChanged(() => Target.PropertyA).MustHaveBeen(Notified.Never);
      a.ShouldThrow<ExpectationException>();
    }

    [TestMethod]
    public void WhenPropertyChangedThenHasNotRecordedEventShouldValidate()
    {
      Target.PropertyA = true;
      Action a = () => tracer.PropertyChanged(() => Target.PropertyA).MustHaveBeen(Notified.Never);
      a.ShouldThrow<ExpectationException>();
    }

    [TestMethod]
    public void WhenCorrectlyWiredPropertyChangedThenHasNotRecordedEventShouldThrow()
    {
      Target.PropertyB = true;

      Action a = () => tracer.PropertyChanged(() => Target.PropertyA).MustHaveBeen(Notified.Never);
      a.ShouldNotThrow();

      Action b = () => tracer.PropertyChanged(() => Target.PropertyB).MustHaveBeen(Notified.Never);
      b.ShouldThrow<ExpectationException>();

      tracer.PropertyChanged(() => Target.PropertyB).AtLeastOnce().Should().Be(true);
    }
  }
}
