namespace InpcTracer.Output
{
  using InpcTracer.Tracing;

  /// <summary>
  /// Provides string formatting for notifications.
  /// </summary>
  internal interface INotificationFormatter
  {
    /// <summary>
    /// Gets a human readable description of the specified notification.
    /// </summary>
    /// <param name="notification">The notification to get a description for.</param>
    /// <returns>A description of the notification.</returns>
    string GetDescription(INotification notification);
  }
}