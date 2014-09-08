namespace InpcTracer.Tracing
{
  using System;

  /// <summary>
  /// Records monitored events.
  /// </summary>
  public class MonitoredEvent : IMonitoredEvent
  {
    /// <summary>
    /// Initialises a new instance of the <see cref="MonitoredEvent" /> class.
    /// </summary>
    /// <param name="name">Name of the event member.</param>
    /// <param name="eventArgs">EventArgs passed by the event.</param>
    public MonitoredEvent(string name, EventArgs eventArgs)
    {
      this.PropertyName = name;
      this.EventArgs = eventArgs;
    }

    /// <summary>
    /// Gets the name of the event member.
    /// </summary>
    public string PropertyName { get; private set; }

    /// <summary>
    /// Gets the eventArgs passed by the event.
    /// </summary>
    public EventArgs EventArgs { get; private set; }
  }
}