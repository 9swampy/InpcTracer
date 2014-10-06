namespace InpcTracer.NTests
{
  using System;
  using System.ComponentModel;
  using FakeItEasy;
  using FluentAssertions;
  using InpcTracer.Tracing;
  using NUnit.Framework;
  
  [TestFixture]
  public class GivenAnIncorrectlyWiredTarget
  {
    private static readonly IExampleNotifyPropertyChanged target;
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

    [SetUp]
    public void TestInitialize()
    {
      tracer = new InpcTracer.InpcTracer<IExampleNotifyPropertyChanged>(target, new ExpressionValidator());
    }

    [Test]
    public void WhenNoPropertyChangedThenHasRecordedEventShouldRaiseAnError()
    {
      Action a = () => tracer.PropertyChanged(() => target.PropertyA).MustHaveOccurred();
      a.ShouldThrow<InpcTracer.Framework.ExpectationException>();
    }

    [Test]
    public void WhenIncorrectlyWiredPropertyChangedThenHasRecordedEventShouldRaiseAnError()
    {
      target.PropertyB = true;

      Action a = () => tracer.PropertyChanged(() => target.PropertyB).MustHaveOccurred();
      a.ShouldThrow<InpcTracer.Framework.ExpectationException>();
    }

    [Test]
    public void WhenOtherPropertyChangedThenFirstRecordedEventShouldThrow()
    {
      target.PropertyA = true;

      Action a = () => tracer.FirstPropertyChanged(() => target.PropertyB);
      a.ShouldThrow<ArgumentException>();
    }

    [Test]
    public void WhenIncorrectlyWiredPropertyChangedThenFirstRecordedEventShouldThrow()
    {
      target.PropertyB = true;

      Action a = () => tracer.FirstPropertyChanged(() => target.PropertyB);
      a.ShouldThrow<ArgumentException>();
    }

    [Test]
    public void WhenSinglePropertyChangedThenSubsequentThenRecordedEventShouldThrow()
    {
      target.PropertyA = true;

      Action a = () => tracer.FirstPropertyChanged(() => target.PropertyA);
      a.ShouldNotThrow();

      Action b = () => tracer.FirstPropertyChanged(() => target.PropertyA).ThenPropertyChanged(() => target.PropertyB);
      b.ShouldThrow<ArgumentException>();
    }

    [Test]
    public void WhenDoublePropertiesChangedThenSubsequentThenRecordedEventShouldThrowAsIncorrectlyWired()
    {
      target.PropertyA = true;
      target.PropertyB = true;

      Action a = () => tracer.FirstPropertyChanged(() => target.PropertyA);
      a.ShouldNotThrow();

      Action b = () => tracer.FirstPropertyChanged(() => target.PropertyA).ThenPropertyChanged(() => target.PropertyB);
      b.ShouldThrow<ArgumentException>();
    }

    [Test]
    public void WhenIncorrectlyWiredPropertyChangedThenHasNotRecordedEventShouldNotThrow()
    {
      target.PropertyB = true;

      Action a = () => tracer.PropertyChanged(() => target.PropertyA).MustHaveBeen(Raised.Never);
      a.ShouldNotThrow();

      Action b = () => tracer.PropertyChanged(() => target.PropertyB).MustHaveBeen(Raised.Never);
      b.ShouldNotThrow();

      tracer.PropertyChanged(() => target.PropertyB).AtLeastOnce().Should().Be(false);
    }
  }
}
