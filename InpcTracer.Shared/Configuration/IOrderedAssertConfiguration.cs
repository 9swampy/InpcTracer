namespace InpcTracer.Configuration
{
  using System;
  using System.Diagnostics.CodeAnalysis;
  using System.Linq.Expressions;

  /// <summary>
  /// Allows the developer to assert on an ordered chain of notifications configured.
  /// </summary>
  public interface IOrderedAssertConfiguration : IAssertConfiguration
  {
    /// <summary>
    /// Assert that the next notification in the chain matches the specified property.
    /// </summary>
    /// <typeparam name="TResult">Type of the relevant property.</typeparam>
    /// <param name="expression">A function that produces the relevant property.</param>
    /// <returns>The next IOrderedAssertConfiguration in the notification chain.</returns>
    [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "This is by design when using the Expression-, Action- and Func-types.")]
    IOrderedAssertConfiguration ThenPropertyChanged<TResult>(Expression<Func<TResult>> expression);
  }
}