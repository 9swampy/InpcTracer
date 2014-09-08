namespace InpcTracer.NTests.Framework
{
  using System;
  using FluentAssertions;
  using NUnit.Framework;

  [TestFixture]
  public class GuardTests
  {
    [Test]
    public void ExceptionMessageShouldWriteThatArgumentCannotBeNullWhenExpressionIsNull()
    {
      var expectedMessage =
        @"Value cannot be null.
Parameter name: name";

      Action act = () => InpcTracer.Framework.Guard.AgainstNull(null, "name");

      act.ShouldThrow<ArgumentNullException>().WithMessage(expectedMessage);
    }
  }
}