namespace InpcTracer.Tests
{
  using InpcTracer.Tracing;
  using Microsoft.VisualStudio.TestTools.UnitTesting;
  using Shouldly;
  using System;

  [TestClass]
  public class GivenAnExpressionValidator
  {
    [TestMethod]
    public void WhenNullExpressionPassedInToValidateAsMemberThenShouldThrow()
    {
      IExpressionValidator expressionValidator = new ExpressionValidator();
      Should.Throw<ArgumentNullException>(() => expressionValidator.ValidateAsMember<bool>(null));
    }

    [TestMethod]
    public void WhenNonMemberExpressionPassedInToValidateAsMemberThenShouldThrow()
    {
      IExpressionValidator expressionValidator = new ExpressionValidator();
      Should.Throw<ArgumentException>(() => expressionValidator.ValidateAsMember<IExpressionValidator>(() => expressionValidator));
    }
  }
}