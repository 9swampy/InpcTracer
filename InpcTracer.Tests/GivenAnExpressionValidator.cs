namespace InpcTracer.Tests
{
  using System;
  using FluentAssertions;
  using InpcTracer.Tracing;
  using Microsoft.VisualStudio.TestTools.UnitTesting;

  [TestClass]
  public class GivenAnExpressionValidator
  {
    [TestMethod]
    public void WhenNullExpressionPassedInToValidateAsMemberThenShouldThrow()
    {
      IExpressionValidator expressionValidator = new ExpressionValidator();
      Action a = () => expressionValidator.ValidateAsMember<bool>(null);
      a.ShouldThrow<ArgumentNullException>();
    }

    [TestMethod]
    public void WhenNonMemberExpressionPassedInToValidateAsMemberThenShouldThrow()
    {
      IExpressionValidator expressionValidator = new ExpressionValidator();

      Action a = () => expressionValidator.ValidateAsMember<IExpressionValidator>(() => expressionValidator);
      a.ShouldThrow<ArgumentException>();
    }
  }
}