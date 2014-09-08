namespace InpcTracer.NTests.Tracing
{
  using System;
  using System.Collections.Generic;
  using System.Reflection;
  using FakeItEasy;
  using FluentAssertions;
  using InpcTracer.Tracing;
  using NUnit.Framework;

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
    public void ExceptionMessageShouldWriteThatArgumentCannotBeNullWhenExpressionIsNull()
    {
      var expectedMessage =
        @"Value cannot be null.
Parameter name: expression";

      Action act = () => this.expressionValidator.ValidateAsMember<IExpressionValidator>(null);
      
      act.ShouldThrow<ArgumentNullException>().WithMessage(expectedMessage);
    }

    [Test]
    public void ExceptionMessageShouldWriteThatArgumentMustBeAPropertyWhenItIsNot()
    {
      var expectedMessage =
        @"The expression must be a property";

      Action act = () => this.expressionValidator.ValidateAsMember<IExpressionValidator>(() => this.expressionValidator);

      act.ShouldThrow<ArgumentException>().WithMessage(expectedMessage);
    }

    [Test]
    public void ShouldValidateAndReturnMatchingMemberWhenMemberIsAProperty()
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
  }
}