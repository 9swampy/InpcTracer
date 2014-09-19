namespace InpcTracer.Tracing
{
  using System;

  /// <summary>
  /// Representation of a recorded notification.
  /// </summary>
  public interface INotification
  {
    /// <summary>
    /// Gets the name of the property changed.
    /// </summary>
    string PropertyName { get; }

    /// <summary>
    /// Centralised equivalency check.
    /// </summary>
    /// <param name="memberExpression">Expression identifying the name of the member to check for.</param>
    /// <returns>True if the name matches.</returns>
    bool MemberNameMatches(string memberExpression);
  }
}