namespace InpcTracer.NTests.Output
{
  using InpcTracer.Tracing;
  using System;
  using System.Collections.Generic;
  using System.Globalization;
  using System.Linq;
  using NUnit.Framework;
  using FakeItEasy;
  using InpcTracer.Output;

  [TestFixture]
  public class NotificationWriterTests
  {
    private List<INotification> calls;
    private StringBuilderOutputWriter writer;

    [Fake]
    public IEqualityComparer<INotification> CallComparer { get; set; }

    [Fake]
    internal INotificationFormatter CallFormatter { get; set; }

    [SetUp]
    public void Setup()
    {
      Fake.InitializeFixture(this);

      A.CallTo(() => this.CallFormatter.GetDescription(A<INotification>._))
          .Returns("Default notification description");

      this.calls = new List<INotification>();
      this.writer = new StringBuilderOutputWriter();
    }

    [Test]
    public void WriteNotifications_should_list_the_calls_in_the_calls_collection()
    {
      this.StubCalls(10);

      int callNumber = 1;
      foreach (var call in this.calls)
      {
        var boundCallNumber = callNumber;
        A.CallTo(() => this.CallFormatter.GetDescription(call)).Returns("Fake notification " + boundCallNumber.ToString(CultureInfo.CurrentCulture));
        callNumber++;
      }

      var callWriter = this.CreateWriter();
      callWriter.WriteNotifications(this.calls, this.writer);

      var message = this.writer.Builder.ToString();
      var expectedMessage =
@"1:  Fake notification 1
2:  Fake notification 2
3:  Fake notification 3
4:  Fake notification 4
5:  Fake notification 5
6:  Fake notification 6
7:  Fake notification 7
8:  Fake notification 8
9:  Fake notification 9
10: Fake notification 10";

      Assert.That(message, Is.StringContaining(expectedMessage));
    }

    [Test]
    public void WriteNotifications_should_skip_duplicate_calls_in_row()
    {
      // Arrange
      this.StubCalls(10);

      A.CallTo(() => this.CallFormatter.GetDescription(A<INotification>._)).Returns("Fake notification");
      A.CallTo(() => this.CallFormatter.GetDescription(this.calls[9])).Returns("Other notification");

      A.CallTo(() => this.CallComparer.Equals(A<INotification>.That.Not.IsEqualTo(this.calls[9]), A<INotification>.That.Not.IsEqualTo(this.calls[9]))).Returns(true);

      var callWriter = this.CreateWriter();

      // Act
      callWriter.WriteNotifications(this.calls, this.writer);

      // Assert
      var message = this.writer.Builder.ToString();
      var expectedMessage =
@"1:  Fake notification repeated 9 times
...
10: Other notification";

      Assert.That(message, Is.StringContaining(expectedMessage));
    }

    [Test]
    public void WriteNotifications_should_not_skip_duplicate_messages_that_are_not_in_row()
    {
      this.StubCalls(4);

      foreach (var call in this.calls.Where((x, i) => i % 2 == 0))
      {
        A.CallTo(() => this.CallFormatter.GetDescription(call)).Returns("odd");
      }

      foreach (var call in this.calls.Where((x, i) => i % 2 != 0))
      {
        A.CallTo(() => this.CallFormatter.GetDescription(call)).Returns("even");
      }

      var callWriter = this.CreateWriter();
      callWriter.WriteNotifications(this.calls, this.writer);

      var message = this.writer.Builder.ToString();
      var expectedMessage =
@"1: odd
2: even
3: odd
4: even";

      Assert.That(message, Is.StringContaining(expectedMessage));
    }

    [Test]
    public void WriteNotifications_should_truncate_calls_list_when_more_than_a_hundred_call_lines_are_printed()
    {
      this.StubCalls(30);

      foreach (var call in this.calls)
      {
        A.CallTo(() => this.CallFormatter.GetDescription(call)).Returns(string.Format(CultureInfo.InvariantCulture, "Fake notification {0}", Guid.NewGuid()));
      }

      A.CallTo(() => this.CallFormatter.GetDescription(this.calls[18])).Returns("Last notification");

      var callWriter = this.CreateWriter();
      callWriter.WriteNotifications(this.calls, this.writer);

      var message = this.writer.Builder.ToString();
      var expectedMessage =
@"19: Last notification
... Found 11 more notifications not displayed here.";

      Assert.That(message, Is.StringContaining(expectedMessage));
    }

    [Test]
    public void WriteNotifications_should_indent_values_with_new_lines_correctly()
    {
      // Arrange
      this.StubCalls(10);

      var text =
@"first line
second line";

      var callIndex = 0;
      A.CallTo(() => this.CallFormatter.GetDescription(A<INotification>._)).ReturnsLazily(() => text + ++callIndex);

      var callWriter = this.CreateWriter();

      // Act
      using (this.writer.Indent())
      {
        callWriter.WriteNotifications(this.calls, this.writer);
      }

      // Assert
      var message = this.writer.Builder.ToString();

      var expectedText1 =
@"1:  first line
    second line";

      var expectedText2 =
@"10: first line
    second line";

      Assert.That(message, Is.StringContaining(expectedText1).And.StringContaining(expectedText2));
    }

    [Test]
    public void WriteNotifications_should_write_new_line_at_end()
    {
      // Arrange
      this.StubCalls(1);

      var callWriter = this.CreateWriter();

      // Act
      callWriter.WriteNotifications(this.calls, this.writer);

      // Assert
      Assert.That(this.writer.Builder.ToString(), Is.StringEnding(Environment.NewLine));
    }

    [Test]
    public void Should_write_nothing_if_call_list_is_empty()
    {
      // Arrange
      var callWriter = this.CreateWriter();

      // Act
      callWriter.WriteNotifications(Enumerable.Empty<INotification>(), this.writer);

      // Assert
      Assert.That(this.writer.Builder.ToString(), Is.Empty);
    }

    private NotificationWriter CreateWriter()
    {
      return new NotificationWriter(this.CallFormatter, this.CallComparer);
    }

    private void StubCalls(int numberOfCalls)
    {
      for (int i = 0; i < numberOfCalls; i++)
      {
        this.calls.Add(A.Fake<INotification>());
      }
    }
  }
}
