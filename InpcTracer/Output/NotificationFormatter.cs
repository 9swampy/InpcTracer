namespace InpcTracer.Output
{
  using InpcTracer.Framework;
  using InpcTracer.Tracing;

  /// <summary>
  /// Provides string formatting for notifications.
  /// </summary>
  public class NotificationFormatter : INotificationFormatter
  {
    /// <summary>
    /// Gets a human readable description of the specified notification.
    /// </summary>
    /// <param name="notification">The notification to get a description for.</param>
    /// <returns>A description of the notification.</returns>
    public string GetDescription(INotification notification)
    {
      Guard.AgainstNull(notification, "notification");

      return notification.PropertyName;
    }
  }
}