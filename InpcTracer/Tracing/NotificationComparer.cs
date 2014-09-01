namespace InpcTracer.Tracing
{
  using System.Collections.Generic;
  using InpcTracer.Framework;

  public class NotificationComparer : IEqualityComparer<INotification>
  {
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