namespace InpcTracer.NTests.Framework
{
  using FluentAssertions;
  using InpcTracer.NTests.TestHelpers;
  using NUnit.Framework;
  using System;

  [TestFixture]
  public class GuardTests
  {
    [Test]
    public void Exception_message_should_write_that_argument_cannot_be_null_when_expression_is_null()
    {
      var message = this.GetExceptionMessage<ArgumentNullException>(() => InpcTracer.Framework.Guard.AgainstNull(null, "name"));

      var expectedMessage =
        @"Value cannot be null.
Parameter name: name";

      Assert.That(message, Is.StringContaining(expectedMessage));
    }

    private string GetExceptionMessage<T>(Action failingAssertion)
    {
      var exception = Record.Exception(failingAssertion);
      exception.Should().NotBeNull();
      exception.Should().BeOfType<T>();
      return exception.Message;
    }
  }
}