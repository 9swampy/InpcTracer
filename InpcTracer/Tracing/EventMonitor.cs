namespace InpcTracer.Tracing
{
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using InpcTracer.Configuration;

  /// <summary>
  /// Records events raised by the monitored object.
  /// </summary>
  /// <typeparam name="T">Type of the monitored object.</typeparam>
  public class EventMonitor<T>
  {
    private readonly IList<IMonitoredEvent> recordedEventList = new List<IMonitoredEvent>();

    private readonly T monitoredObject;

    /// <summary>
    /// Initialises a new instance of the <see cref="EventMonitor{T}" /> class.
    /// </summary>
    /// <param name="monitoredObject">The object to monitor.</param>
    public EventMonitor(T monitoredObject)
    {
      this.monitoredObject = monitoredObject;
      this.Attach();
    }

    /// <summary>
    /// Clear the recorded event list.
    /// </summary>
    public void Reset()
    {
      this.recordedEventList.Clear();
    }

    /// <summary>
    /// Generate a configuration to assert if the specified property has been notified.
    /// </summary>
    /// <param name="eventName">Name of the relevant event.</param>
    /// <returns>Assert configuration for the specified event.</returns>
    public IAssertConfiguration Event(string eventName)
    {
      if (this.monitoredObject.GetType().GetEvents().Any(o => o.Name == eventName))
      {
        return new AssertConfiguration(this.recordedEventList.OfType<INotification>().ToList(), eventName);
      }
      else
      {
        throw new ArgumentException("The parameter must specify an event.");
      }
    }

    /// <summary>
    /// Convert the original delegate to the required type.
    /// </summary>
    /// <param name="originalDelegate">The original delegate.</param>
    /// <param name="targetDelegateType">Type of the target delegate.</param>
    /// <returns>Delegate as targetDelegateType.</returns>
    private static Delegate ConvertDelegate(Delegate originalDelegate, Type targetDelegateType)
    {
      // http://stackoverflow.com/questions/3120422/how-to-attach-event-handler-to-an-event-using-reflection
      return Delegate.CreateDelegate(
        targetDelegateType,
        originalDelegate.Target,
        originalDelegate.Method);
    }

    private void Attach()
    {
      if (this.monitoredObject != null)
      {
        foreach (var eventInfo in this.monitoredObject.GetType().GetEvents())
        {
          Action<object, EventArgs> handler = (s, e) =>
          {
            this.recordedEventList.Add(new MonitoredEvent(eventInfo.Name, e));
          };
          Delegate convertedHandler = ConvertDelegate(handler, eventInfo.EventHandlerType);
          eventInfo.AddEventHandler(this.monitoredObject, convertedHandler);
        }
      }
    }
  }
}