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
    /// <exception cref="ExpectationException">The notification has not been called.</exception>
    void MustHaveHappened();
    
    /// <summary>
    /// Asserts that the configured notification has happened the number of times
    /// constrained by the repeatConstraint parameter.
    /// </summary>
    /// <param name="repeatConstraint">A constraint for how many times the notification
    /// must have happened.</param>
    /// <exception cref="ExpectationException">The notification has not been called a number of times
    /// that passes the repeat constraint.</exception>
    void MustHaveHappened(Repeated repeatConstraint);

    bool ExactlyOnce();

    bool AtLeastOnce();
  }
}