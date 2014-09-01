namespace InpcTracer.NTests.Tracing
{
  using FakeItEasy;
  using FluentAssertions;
  using InpcTracer.NTests.TestHelpers;
  using InpcTracer.Tracing;
  using NUnit.Framework;
  using System;
  using System.Collections.Generic;
  using System.Reflection;

  [TestFixture]
  public class ExpressionValidatorTests
  {
    private IExpressionValidator expressionValidator;

    [SetUp]
    public void Setup()
    {
      this.expressionValidator = new ExpressionValidator();
    }

    [Test]
    public void Exception_message_should_write_that_argument_cannot_be_null_when_expression_is_null()
    {
      var message = this.GetExceptionMessage<ArgumentNullException>(() => this.expressionValidator.ValidateAsMember<IExpressionValidator>(null));

      var expectedMessage =
        @"Value cannot be null.
Parameter name: expression";

      Assert.That(message, Is.StringContaining(expectedMessage));
    }

    [Test]
    public void Exception_message_should_write_that_argument_must_be_a_property_when_expression_it_is_not()
    {
      var message = this.GetExceptionMessage<ArgumentException>(() => this.expressionValidator.ValidateAsMember<IExpressionValidator>(() => this.expressionValidator));

      var expectedMessage =
        @"The expression must be a Property";

      Assert.That(message, Is.StringContaining(expectedMessage));
    }

    [Test]
    public void Should_validate_and_return_matching_member_when_member_is_a_property()
    {
      // Arrange
      var objectWithCountProperty = A.Fake<IList<string>>();

      // Act
      var memberExpression = this.expressionValidator.ValidateAsMember(() => objectWithCountProperty.Count);

      //Assert
      Assert.That(memberExpression, Is.Not.Null);
      Assert.That(memberExpression.Member.MemberType, Is.EqualTo(MemberTypes.Property));
      Assert.That(memberExpression.Member.Name, Is.EqualTo("Count"));
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