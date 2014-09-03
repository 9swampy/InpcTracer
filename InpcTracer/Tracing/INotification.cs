namespace InpcTracer.Tracing
{
  using System;

  /// <summary>
  /// Representation of a recorded notification.
  /// </summary>
  public interface INotification
  {
    /// <summary>
    /// Gets the name of the property changed.
    /// </summary>
    string PropertyName { get; }
  }
}