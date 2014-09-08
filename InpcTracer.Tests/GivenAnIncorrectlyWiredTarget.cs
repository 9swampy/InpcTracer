namespace InpcTracer.Tests
{
  using System;
  using System.ComponentModel;
  using FakeItEasy;
  using FluentAssertions;
  using InpcTracer.Configuration;
  using InpcTracer.Tracing;
  using Microsoft.VisualStudio.TestTools.UnitTesting;

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
      Action a = () => tracer.PropertyChanged(() => target.PropertyA).MustHaveOccurred();
      a.ShouldThrow<InpcTracer.Framework.ExpectationException>();
    }

    [TestMethod]
    public void WhenIncorrectlyWiredPropertyChangedThenHasRecordedEventShouldRaiseAnError()
    {
      target.PropertyB = true;

      Action a = () => tracer.PropertyChanged(() => target.PropertyB).MustHaveOccurred();
      a.ShouldThrow<InpcTracer.Framework.ExpectationException>();
    }

    [TestMethod]
    public void WhenOtherPropertyChangedThenFirstRecordedEventShouldThrow()
    {
      target.PropertyA = true;

      Action a = () => tracer.FirstPropertyChanged(() => target.PropertyB);
      a.ShouldThrow<ArgumentException>();
    }

    [TestMethod]
    public void WhenIncorrectlyWiredPropertyChangedThenFirstRecordedEventShouldThrow()
    {
      target.PropertyB = true;

      Action a = () => tracer.FirstPropertyChanged(() => target.PropertyB);
      a.ShouldThrow<ArgumentException>();
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
    public void WhenDoublePropertiesChangedThenSubsequentThenRecordedEventShouldThrowAsIncorrectlyWired()
    {
      target.PropertyA = true;
      target.PropertyB = true;

      Action a = () => tracer.FirstPropertyChanged(() => target.PropertyA);
      a.ShouldNotThrow();

      Action b = () => tracer.FirstPropertyChanged(() => target.PropertyA).ThenPropertyChanged(() => target.PropertyB);
      b.ShouldThrow<ArgumentException>();
    }

    [TestMethod]
    public void WhenIncorrectlyWiredPropertyChangedThenHasNotRecordedEventShouldNotThrow()
    {
      target.PropertyB = true;

      Action a = () => tracer.PropertyChanged(() => target.PropertyA).MustHaveBeen(Notified.Never);
      a.ShouldNotThrow();

      Action b = () => tracer.PropertyChanged(() => target.PropertyB).MustHaveBeen(Notified.Never);
      b.ShouldNotThrow();
      
      tracer.PropertyChanged(() => target.PropertyB).AtLeastOnce().Should().Be(false);
    }
  }
}
