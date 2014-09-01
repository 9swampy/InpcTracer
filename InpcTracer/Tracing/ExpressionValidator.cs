namespace InpcTracer.Tracing
{
  using System;
  using System.Linq.Expressions;

  public class ExpressionValidator : IExpressionValidator
  {
    /// <summary>
    /// Validate the expression as a Member.
    /// </summary>
    /// <typeparam name="T">The type of member property.</typeparam>
    /// <param name="expression">An expression accessing the relevant property.</param>
    /// <returns>MemberExpression defining the property.</returns>
    /// <exception cref="System.ArgumentNullException">The specified argument was null.</exception>
    /// <exception cref="System.ArgumentException">The specified argument must be a property.</exception>
    public MemberExpression ValidateAsMember<T>(Expression<Func<T>> expression)
    {
      if (expression == null)
      {
        throw new ArgumentNullException("expression");
      }

      var result = expression.Body as MemberExpression;
      if (result == null || result.Member.MemberType != System.Reflection.MemberTypes.Property)
      {
        throw new ArgumentException("The expression must be a Property");
      }

      return result;
    }
  }
}