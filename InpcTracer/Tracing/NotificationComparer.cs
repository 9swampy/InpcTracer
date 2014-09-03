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
    /// 
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns></returns>
    public bool Equals(INotification x, INotification y)
    {
      Guard.AgainstNull(x, "x");
      Guard.AgainstNull(y, "y");

      return x.PropertyName.Equals(y.PropertyName);
    }

    public int GetHashCode(INotification obj)
    {
      Guard.AgainstNull(obj, "obj");

      return obj.PropertyName.GetHashCode();
    }
  }
}