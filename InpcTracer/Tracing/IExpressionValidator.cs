namespace InpcTracer.Tracing
{
  using System;
  using System.Diagnostics.CodeAnalysis;
  using System.Linq.Expressions;

  /// <summary>
  /// Validation of expression functions.
  /// </summary>
  public interface IExpressionValidator
  {
    /// <summary>
    /// Validate the expression as a Member.
    /// </summary>
    /// <typeparam name="T">The type of member property.</typeparam>
    /// <param name="expression">An expression accessing the relevant property.</param>
    /// <returns>MemberExpression defining the property.</returns>
    /// <exception cref="System.ArgumentNullException">The specified argument was null.</exception>
    /// <exception cref="System.ArgumentException">The specified argument must be a property.</exception>
    [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "This is by design when using the Expression-, Action- and Func-types.")]
    MemberExpression ValidateAsMember<T>(Expression<Func<T>> expression);
  }
}