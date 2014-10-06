namespace InpcTracer.NTests
{
  using System;
  using FluentAssertions;
  using InpcTracer.Configuration;
  using InpcTracer.Framework;
  using InpcTracer.Tracing;
  using NUnit.Framework;
 
  [TestFixture]
  public class GivenACorrectlyWiredTarget
  {
    private static readonly IExampleNotifyPropertyChanged Target;
    private static InpcTracer.InpcTracer<IExampleNotifyPropertyChanged> tracer;

    static GivenACorrectlyWiredTarget()
    {
      Target = ExampleTargetProvider.ACorrectlyWiredTarget();
    }

    [SetUp]
    public void TestInitialize()
    {
      tracer = new InpcTracer.InpcTracer<IExampleNotifyPropertyChanged>(Target, new ExpressionValidator());
    }

    [Test]
    public void WhenNoPropertyChangedThenHasRecordedEventShouldRaiseAnError()
    {
      Action a = () => tracer.PropertyChanged(() => Target.PropertyA).MustHaveBeen(Raised.Exactly.Once);
      a.ShouldThrow<InpcTracer.Framework.ExpectationException>();
    }

    [Test]
    public void WhenPropertyChangedThenHasRecordedEventShouldNotRaiseAnError()
    {
      Target.PropertyA = true;
      tracer.PropertyChanged(() => Target.PropertyA).MustHaveOccurred();
      tracer.PropertyChanged(() => Target.PropertyA).MustHaveBeen(Raised.Exactly.Once);
      Action a = () => tracer.PropertyChanged(() => Target.PropertyA);
      a.ShouldNotThrow();
      tracer.PropertyChanged(() => Target.PropertyA).ExactlyOnce().Should().Be(true);
      Action b = () => tracer.PropertyChanged(() => Target.PropertyA).MustHaveBeen(Raised.Like(o => o == 1 ? RepeatMatch.SatisfiedPending : RepeatMatch.Unsatisfied));
      b.ShouldNotThrow();
    }

    [Test]
    public void WhenPropertyChangedThenFirstRecordedEventShouldMatch()
    {
      Target.PropertyB = true;
      Action a = () => tracer.FirstPropertyChanged(() => Target.PropertyA);
      a.ShouldThrow<ArgumentException>();
    }

    [Test]
    public void WhenPropertyChangedThenFirstRecordedEventShouldNotThrow()
    {
      Target.PropertyB = true;
      Action a = () => tracer.FirstPropertyChanged(() => Target.PropertyB);
      a.ShouldNotThrow();
    }

    [Test]
    public void WhenSinglePropertyChangedThenSubsequentThenRecordedEventShouldThrow()
    {
      Target.PropertyA = true;
      Action a = () => tracer.FirstPropertyChanged(() => Target.PropertyA);
      a.ShouldNotThrow();
      Action b = () => tracer.FirstPropertyChanged(() => Target.PropertyA).ThenPropertyChanged(() => Target.PropertyB);
      b.ShouldThrow<ArgumentException>();
    }

    [Test]
    public void WhenDoublePropertiesChangedThenSubsequentThenRecordedEventShouldNotThrow()
    {
      Target.PropertyA = true;
      Target.PropertyB = true;
      Action a = () => tracer.FirstPropertyChanged(() => Target.PropertyA);
      a.ShouldNotThrow();
      Action b = () => tracer.FirstPropertyChanged(() => Target.PropertyA).ThenPropertyChanged(() => Target.PropertyB);
      b.ShouldNotThrow();
    }

    [Test]
    public void WhenSinglePropertyChangedThenMustHaveHappendedTwiceShouldThrow()
    {
      Target.PropertyA = true;
      Target.PropertyB = true;
      Action a = () => tracer.FirstPropertyChanged(() => Target.PropertyA);
      a.ShouldNotThrow();
      Action b = () => tracer.FirstPropertyChanged(() => Target.PropertyA).ThenPropertyChanged(() => Target.PropertyB);
      b.ShouldNotThrow();
    }

    [Test]
    public void WhenUnmatchedPropertiesChangedThenMustHaveHappendedTwiceShouldThrow()
    {
      Target.PropertyA = true;
      Target.PropertyB = true;

      Action a = () => tracer.PropertyChanged(() => Target.PropertyA).MustHaveBeen(Raised.AtLeast.Twice);
      a.ShouldThrow<ExpectationException>();
      Action b = () => tracer.PropertyChanged(() => Target.PropertyA).MustHaveBeen(Raised.Exactly.Twice);
      b.ShouldThrow<ExpectationException>();
    }

    [Test]
    public void WhenPropertyChangedThenHasNotRecordedEventShouldThrow()
    {
      Target.PropertyA = true;
      Action a = () => tracer.PropertyChanged(() => Target.PropertyA).MustHaveBeen(Raised.Never);
      a.ShouldThrow<ExpectationException>();
    }

    [Test]
    public void WhenPropertyChangedThenHasNotRecordedEventShouldValidate()
    {
      Target.PropertyA = true;
      Action a = () => tracer.PropertyChanged(() => Target.PropertyA).MustHaveBeen(Raised.Never);
      a.ShouldThrow<ExpectationException>();
    }

    [Test]
    public void WhenCorrectlyWiredPropertyChangedThenHasNotRecordedEventShouldThrow()
    {
      Target.PropertyB = true;

      Action a = () => tracer.PropertyChanged(() => Target.PropertyA).MustHaveBeen(Raised.Never);
      a.ShouldNotThrow();

      Action b = () => tracer.PropertyChanged(() => Target.PropertyB).MustHaveBeen(Raised.Never);
      b.ShouldThrow<ExpectationException>();

      tracer.PropertyChanged(() => Target.PropertyB).AtLeastOnce().Should().Be(true);
    }
  }
}