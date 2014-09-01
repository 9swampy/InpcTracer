namespace InpcTracer.Tracing
{
  using System;

  public interface INotification
  {
    /// <summary>
    /// Gets the name of the property changed.
    /// </summary>
    string PropertyName { get; }
  }
}