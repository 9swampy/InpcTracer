namespace InpcTracer.Tracing
{
  using InpcTracer.Framework;

  /// <summary>
  /// Representation of a recorded notification.
  /// </summary>
  public class Notification : INotification
  {
    /// <summary>
    /// Initialises a new instance of the <see cref="Notification" /> class.
    /// </summary>
    /// <param name="propertyName">Name of the property being notified.</param>
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
    
    /// <summary>
    /// Centralised equivalency check.
    /// </summary>
    /// <param name="memberExpression">Expression identifying the name of the member to check for.</param>
    /// <returns>True if the name matches.</returns>
    public bool MemberNameMatches(string memberExpression)
    {
      return string.Equals(this.PropertyName, memberExpression, System.StringComparison.OrdinalIgnoreCase);
    }
  }
}