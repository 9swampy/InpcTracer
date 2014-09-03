namespace InpcTracer.Configuration
{
  using System.Collections.Generic;
  using System.Linq.Expressions;
  using InpcTracer.Output;
  using InpcTracer.Tracing;

  /// <summary>
  /// Allows the developer to assert on a notification that's configured.
  /// </summary>
  public class AssertConfiguration : IAssertConfiguration
  {
    private readonly IList<INotification> recordedNotifications;
    private readonly MemberExpression memberExpression;

    /// <summary>
    /// Configures an assert for the specified member.
    /// </summary>
    /// <param name="recordedNotifications">Collection of notifications recorded.</param>
    /// <param name="memberExpression">The MemberExpression that produces the relevant property.</param>
    public AssertConfiguration(IList<INotification> recordedNotifications, MemberExpression memberExpression)
    {
      this.recordedNotifications = recordedNotifications;
      this.memberExpression = memberExpression;
    }

    /// <summary>
    /// Whether the notification has occurred exactly once.
    /// </summary>
    /// <returns>True if has occurred exactly once, otherwise false.</returns>
    public bool ExactlyOnce()
    {
      RecordedNotificationAsserter recordedNotificationAsserter = new RecordedNotificationAsserter(this.recordedNotifications, new NotificationWriter(new NotificationFormatter(), new NotificationComparer()));
      return recordedNotificationAsserter.WasRecorded(o => o.PropertyName == this.memberExpression.Member.Name, x => Repeated.Exactly.Once.Matches(x));
    }

    /// <summary>
    /// Whether the notification has occurred at least once.
    /// </summary>
    /// <returns>True if has occurred at least once, otherwise false.</returns>
    public bool AtLeastOnce()
    {
      RecordedNotificationAsserter recordedNotificationAsserter = new RecordedNotificationAsserter(this.recordedNotifications, new NotificationWriter(new NotificationFormatter(), new NotificationComparer()));
      return recordedNotificationAsserter.WasRecorded(o => o.PropertyName == this.memberExpression.Member.Name, x => Repeated.AtLeast.Once.Matches(x));
    }

    /// <summary>
    /// Asserts whether the notification has occurred at least once.
    /// </summary>
    /// <exception cref="InpcTracer.Framework.ExpectationException">The notification has not been called even once.</exception>
    public void MustHaveHappened()
    {
      this.MustHaveHappened(Repeated.AtLeast.Once);
    }

    /// <summary>
    /// Asserts that the configured notification has happened the number of times
    /// constrained by the repeatConstraint parameter.
    /// </summary>
    /// <param name="repeatConstraint">A constraint for how many times the notification
    /// must have happened.</param>
    /// <exception cref="InpcTracer.Framework.ExpectationException">The notification has not been called a number of times
    /// that passes the repeat constraint.</exception>
    public void MustHaveHappened(Repeated repeatConstraint)
    {
      RecordedNotificationAsserter recordedNotificationAsserter = new RecordedNotificationAsserter(this.recordedNotifications, new NotificationWriter(new NotificationFormatter(), new NotificationComparer()));
      recordedNotificationAsserter.AssertWasRecorded(o => o.PropertyName == this.memberExpression.Member.Name, this.memberExpression.Member.Name, x => repeatConstraint.Matches(x), repeatConstraint.ToString());
    }
  }
}