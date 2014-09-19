namespace InpcTracer.Configuration
{
  /// <summary>
  /// Allows the developer to assert on a notification that's configured.
  /// </summary>
  public interface IAssertConfiguration
  : IHideObjectMembers
  {
    /// <summary>
    /// Asserts that the configured notification has happened at least once.
    /// </summary>
    /// <exception cref="InpcTracer.Framework.ExpectationException">The notification has not been called.</exception>
    void MustHaveOccurred();
    
    /// <summary>
    /// Asserts that the configured notification has happened the number of times
    /// constrained by the repeatConstraint parameter.
    /// </summary>
    /// <param name="repeatConstraint">A constraint for how many times the notification
    /// must have happened.</param>
    /// <exception cref="InpcTracer.Framework.ExpectationException">The notification has not been called a number of times
    /// that passes the repeat constraint.</exception>
    void MustHaveBeen(Raised repeatConstraint);

    /// <summary>
    /// Whether the notification has occurred exactly once.
    /// </summary>
    /// <returns>True if has occurred exactly once, otherwise false.</returns>
    bool ExactlyOnce();

    /// <summary>
    /// Whether the notification has occurred at least once.
    /// </summary>
    /// <returns>True if has occurred at least once, otherwise false.</returns>
    bool AtLeastOnce();
  }
}