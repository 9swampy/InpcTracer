namespace InpcTracer.Tracing
{
  using System;
  using System.Diagnostics.CodeAnalysis;
  using System.Linq.Expressions;

  public interface IExpressionValidator
  {
    [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "This is by design when using the Expression-, Action- and Func-types.")]
    MemberExpression ValidateAsMember<T>(Expression<Func<T>> expression);
  }
}