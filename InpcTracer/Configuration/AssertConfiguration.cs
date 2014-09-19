namespace InpcTracer.Configuration
{
  using System.Collections.Generic;
  using InpcTracer.Output;
  using InpcTracer.Tracing;

  /// <summary>
  /// Allows the developer to assert on a notification that's configured.
  /// </summary>
  public class AssertConfiguration : IAssertConfiguration
  {
    private readonly IList<INotification> recordedNotifications;
    private readonly string memberExpression;
    private readonly int timeout;

    /// <summary>
    /// Initialises a new instance of the <see cref="AssertConfiguration" /> class.    
    /// </summary>
    /// <param name="recordedNotifications">Collection of notifications recorded.</param>
    /// <param name="memberExpression">The MemberExpression that produces the relevant property.</param>
    public AssertConfiguration(IList<INotification> recordedNotifications, string memberExpression)
      : this(recordedNotifications, memberExpression, 0)
    {
    }

    /// <summary>
    /// Initialises a new instance of the <see cref="AssertConfiguration" /> class.    
    /// </summary>
    /// <param name="recordedNotifications">Collection of notifications recorded.</param>
    /// <param name="memberExpression">The MemberExpression that produces the relevant property.</param>
    /// <param name="timeout">Maximum milliseconds to wait for events to occur.</param>
    public AssertConfiguration(IList<INotification> recordedNotifications, string memberExpression, int timeout)
    {
      this.timeout = timeout;
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
      return recordedNotificationAsserter.WasRecorded(o => o.MemberNameMatches(this.memberExpression), x => Notified.Exactly.Once.Matches(x));
    }

    /// <summary>
    /// Whether the notification has occurred at least once.
    /// </summary>
    /// <returns>True if has occurred at least once, otherwise false.</returns>
    public bool AtLeastOnce()
    {
      RecordedNotificationAsserter recordedNotificationAsserter = new RecordedNotificationAsserter(this.recordedNotifications, new NotificationWriter(new NotificationFormatter(), new NotificationComparer()));
      return recordedNotificationAsserter.WasRecorded(o => o.MemberNameMatches(this.memberExpression), x => Notified.AtLeast.Once.Matches(x));
    }

    /// <summary>
    /// Asserts whether the notification has occurred at least once.
    /// </summary>
    /// <exception cref="InpcTracer.Framework.ExpectationException">The notification has not been called even once.</exception>
    public void MustHaveOccurred()
    {
      this.MustHaveBeen(Notified.AtLeast.Once);
    }

    /// <summary>
    /// Asserts that the configured notification has happened the number of times
    /// constrained by the repeatConstraint parameter.
    /// </summary>
    /// <param name="repeatConstraint">A constraint for how many times the notification
    /// must have happened.</param>
    /// <exception cref="InpcTracer.Framework.ExpectationException">The notification has not been called a number of times
    /// that passes the repeat constraint.</exception>
    public void MustHaveBeen(Notified repeatConstraint)
    {
      RecordedNotificationAsserter recordedNotificationAsserter = new RecordedNotificationAsserter(this.recordedNotifications, new NotificationWriter(new NotificationFormatter(), new NotificationComparer()));
      recordedNotificationAsserter.AssertWasRecorded(o => o.MemberNameMatches(this.memberExpression), this.memberExpression, x => repeatConstraint.Matches(x), repeatConstraint.ToString(), this.timeout);
    }
  }
}