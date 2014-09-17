namespace InpcTracer.Tracing
{
  using System;
  using System.Linq;

  /// <summary>
  /// Records monitored events.
  /// </summary>
  public interface IMonitoredEvent : INotification
  {
    /// <summary>
    /// Gets the eventArgs passed by the event.
    /// </summary>
    EventArgs EventArgs { get; }
  }
}