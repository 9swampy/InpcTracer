namespace InpcTracer.NTests.Output
{
  using System;
  using System.Collections.Generic;
  using FakeItEasy;
  using FluentAssertions;
  using InpcTracer.NTests;
  using InpcTracer.NTests.TestHelpers;
  using InpcTracer.Output;
  using InpcTracer.Tracing;
  using NUnit.Framework;

  [TestFixture]
  public class RecordedNotificationAsserterTests
  {
    private List<INotification> calls;
    private NotificationWriter callWriter;

    [SetUp]
    public void Setup()
    {
      this.calls = new List<INotification>();
      this.callWriter = A.Fake<NotificationWriter>();
    }

    [Test]
    public void AssertWasCalledShouldPassWhenTheRepeatPredicateReturnsTrueForTheNumberOfMatchingCalls()
    {
      this.StubCalls(2);

      var asserter = this.CreateAsserter();

      asserter.AssertWasRecorded(x => true, string.Empty, x => x == 2, string.Empty);
    }

    [Test]
    public void AssertWasCalledShouldFailWhenTheRepeatPredicateReturnsFalseForTheNumberOfMatchingCalls()
    {
      this.StubCalls(2);

      var asserter = this.CreateAsserter();

      Assert.Throws<InpcTracer.Framework.ExpectationException>(() => asserter.AssertWasRecorded(x => true, string.Empty, x => false, string.Empty));
    }

    [Test]
    public void AssertWasCalledShouldPassTheNumberOfMatchingCallsToTheRepeatPredicate()
    {
      int? numberPassedToRepeatPredicate = null;

      this.StubCalls(4);

      var asserter = this.CreateAsserter();

      asserter.AssertWasRecorded(x => this.calls.IndexOf(x) == 0, string.Empty, x => { numberPassedToRepeatPredicate = x; return true; }, string.Empty);

      Assert.That(numberPassedToRepeatPredicate, Is.EqualTo(1));
    }

    [Test]
    public void ExceptionMessageShouldStartWithCallSpecification()
    {
      var asserter = this.CreateAsserter();

      var message = this.GetExceptionMessage(() =>
          asserter.AssertWasRecorded(x => true, @"IFoo.Bar(1)", x => false, string.Empty));
      var expectedMessage =
@"

  Assertion failed for the following PropertyChanged notification:
    IFoo.Bar(1)";

      Assert.That(message, Is.StringStarting(expectedMessage));
    }

    [Test]
    public void ExceptionMessageShouldWriteRepeatExpectation()
    {
      this.StubCalls(2);

      var asserter = this.CreateAsserter();

      var message = this.GetExceptionMessage(() =>
          asserter.AssertWasRecorded(x => false, string.Empty, x => x == 2, "#2 times"));

      var expectedMessage =
@"
  Expected to find it #2 times but found it #0 times among the notifications:";

      Assert.That(message, Is.StringContaining(expectedMessage));
    }

    [Test]
    public void ExceptionMessageShouldCallTheCallWriterToAppendCallsList()
    {
      this.StubCalls(2);

      var asserter = this.CreateAsserter();

      this.GetExceptionMessage(() => asserter.AssertWasRecorded(x => false, string.Empty, x => false, string.Empty));

      A.CallTo(() => this.callWriter.WriteNotifications(A<IEnumerable<INotification>>.That.IsThisSequence(this.calls), A<InpcTracer.Output.IOutputWriter>._)).MustHaveHappened();
    }

    [Test]
    public void ExceptionMessageShouldWriteThatNoCallsWereMadeWhenCallsIsEmpty()
    {
      this.calls.Clear();

      var asserter = this.CreateAsserter();

      var message = this.GetExceptionMessage(() =>
          asserter.AssertWasRecorded(x => false, string.Empty, x => x == 2, "#2 times"));

      var expectedMessage =
@"
  Expected to find it #2 times but no notifications were recorded.";

      Assert.That(message, Is.StringContaining(expectedMessage));
    }

    [Test]
    public void ExceptionMessageShouldEndWithBlankLine()
    {
      var asserter = this.CreateAsserter();

      var message = this.GetExceptionMessage(() =>
          asserter.AssertWasRecorded(x => false, string.Empty, x => false, string.Empty));

      Assert.That(message, Is.StringEnding(string.Concat(Environment.NewLine, Environment.NewLine)));
    }

    [Test]
    public void ExceptionMessageShouldStartWithBlankLine()
    {
      var asserter = this.CreateAsserter();

      var message = this.GetExceptionMessage(() =>
          asserter.AssertWasRecorded(x => false, string.Empty, x => false, string.Empty));

      Assert.That(message, Is.StringStarting(Environment.NewLine));
    }

    private RecordedNotificationAsserter CreateAsserter()
    {
      return new RecordedNotificationAsserter(this.calls, this.callWriter);
    }

    private void StubCalls(int numberOfCalls)
    {
      for (int i = 0; i < numberOfCalls; i++)
      {
        this.calls.Add(A.Fake<INotification>());
      }
    }

    private string GetExceptionMessage(Action failingAssertion)
    {
      var exception = Record.Exception(failingAssertion);
      exception.Should().NotBeNull();
      exception.Should().BeOfType<InpcTracer.Framework.ExpectationException>();
      return exception.Message;
    }
  }
}