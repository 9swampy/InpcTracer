namespace InpcTracer.Tracing
{
  using InpcTracer.Framework;

  /// <summary>
  /// Representation of a recorded notification.
  /// </summary>
  public class Notification : INotification
  {
    /// <summary>
    /// Create a representation of a recorded notification.
    /// </summary>
    public Notification(string propertyName)
    {
      Guard.AgainstNull(propertyName, "propertyName");

      this.PropertyName = propertyName;
    }

    /// <summary>
    /// Gets the name of the property changed.
    /// </summary>
    public string PropertyName
    {
      get;
      private set;
    }
  }
}