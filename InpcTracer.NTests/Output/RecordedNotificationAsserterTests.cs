﻿namespace InpcTracer.NTests.Output
{
  using InpcTracer.Output;
  using System;
  using System.Collections.Generic;
  using InpcTracer.Framework;
  using InpcTracer.NTests;
  using InpcTracer.NTests.TestHelpers;
  using FluentAssertions;
  using NUnit.Framework;
  using InpcTracer.Tracing;
  using FakeItEasy;

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
    public void AssertWasCalled_should_pass_when_the_repeatPredicate_returns_true_for_the_number_of_matching_calls()
    {
      this.StubCalls(2);

      var asserter = this.CreateAsserter();

      asserter.AssertWasRecorded(x => true, string.Empty, x => x == 2, string.Empty);
    }

    [Test]
    public void AssertWasCalled_should_fail_when_the_repeatPredicate_returns_false_for_the_number_of_matching_calls()
    {
      this.StubCalls(2);

      var asserter = this.CreateAsserter();

      Assert.Throws<InpcTracer.Framework.ExpectationException>(() => asserter.AssertWasRecorded(x => true, string.Empty, x => false, string.Empty));
    }

    [Test]
    public void AssertWasCalled_should_pass_the_number_of_matching_calls_to_the_repeatPredicate()
    {
      int? numberPassedToRepeatPredicate = null;

      this.StubCalls(4);

      var asserter = this.CreateAsserter();

      asserter.AssertWasRecorded(x => this.calls.IndexOf(x) == 0, string.Empty, x => { numberPassedToRepeatPredicate = x; return true; }, string.Empty);

      Assert.That(numberPassedToRepeatPredicate, Is.EqualTo(1));
    }

    [Test]
    public void Exception_message_should_start_with_call_specification()
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
    public void Exception_message_should_write_repeat_expectation()
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
    public void Exception_message_should_call_the_call_writer_to_append_calls_list()
    {
      this.StubCalls(2);

      var asserter = this.CreateAsserter();

      this.GetExceptionMessage(() => asserter.AssertWasRecorded(x => false, string.Empty, x => false, string.Empty));

      A.CallTo(() => this.callWriter.WriteNotifications(A<IEnumerable<INotification>>.That.IsThisSequence(this.calls), A<InpcTracer.Output.IOutputWriter>._)).MustHaveHappened();
    }

    [Test]
    public void Exception_message_should_write_that_no_calls_were_made_when_calls_is_empty()
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
    public void Exception_message_should_end_with_blank_line()
    {
      var asserter = this.CreateAsserter();

      var message = this.GetExceptionMessage(() =>
          asserter.AssertWasRecorded(x => false, string.Empty, x => false, string.Empty));

      Assert.That(message, Is.StringEnding(string.Concat(Environment.NewLine, Environment.NewLine)));
    }

    [Test]
    public void Exception_message_should_start_with_blank_line()
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