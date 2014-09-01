namespace InpcTracer.Output
{
  using InpcTracer.Framework;
  using InpcTracer.Tracing;

  public class NotificationFormatter : INotificationFormatter
  {
    public string GetDescription(INotification notification)
    {
      Guard.AgainstNull(notification, "notification");

      return notification.PropertyName;
    }
  }
}