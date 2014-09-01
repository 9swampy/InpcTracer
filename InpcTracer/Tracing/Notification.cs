namespace InpcTracer.Tracing
{
  using InpcTracer.Framework;

  public class Notification : INotification
  {
    public Notification(string propertyName)
    {
      Guard.AgainstNull(propertyName, "propertyName");

      this.PropertyName = propertyName;
    }

    public string PropertyName
    {
      get;
      private set;
    }
  }
}