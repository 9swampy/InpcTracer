namespace InpcTracer.Tracing
{
  using System.Collections.Generic;
  using InpcTracer.Framework;

  /// <summary>
  /// Comparison of notifications.
  /// </summary>
  internal class NotificationComparer : IEqualityComparer<INotification>
  {
    /// <summary>
    /// Compares two instances of INotification.
    /// </summary>
    /// <param name="x">First element.</param>
    /// <param name="y">Other element.</param>
    /// <returns>True if equal, otherwise false.</returns>
    public bool Equals(INotification x, INotification y)
    {
      Guard.AgainstNull(x, "x");
      Guard.AgainstNull(y, "y");

      return x.PropertyName.Equals(y.PropertyName);
    }

    /// <summary>
    /// Calculate a hash code for INotification.
    /// </summary>
    /// <param name="obj">The object to hash.</param>
    /// <returns>Hash code for object.</returns>
    public int GetHashCode(INotification obj)
    {
      Guard.AgainstNull(obj, "obj");

      return obj.PropertyName.GetHashCode();
    }
  }
}