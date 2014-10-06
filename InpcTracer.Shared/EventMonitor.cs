namespace InpcTracer
{
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using System.Reflection;
  using InpcTracer.Configuration;
  using InpcTracer.Tracing;

  /// <summary>
  /// Records events raised by the monitored object.
  /// </summary>
  /// <typeparam name="T">Type of the monitored object.</typeparam>
  public class EventMonitor<T>
  {
#if !Universal81
    private const BindingFlags RelevantBindingFlags = BindingFlags.FlattenHierarchy | BindingFlags.IgnoreCase | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
#endif
    private static volatile object recordedEventListSynchLock = new object();
    private readonly IList<INotification> recordedEventList = new List<INotification>();

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
      lock (recordedEventListSynchLock)
      {
        this.recordedEventList.Clear();
      }
    }

    /// <summary>
    /// Generate a configuration to assert if the specified property has been notified.
    /// </summary>
    /// <param name="eventName">Name of the relevant event.</param>
    /// <returns>Assert configuration for the specified event.</returns>
    public IAssertConfiguration Event(string eventName)
    {
      return this.Event(eventName, 0);
    }

    /// <summary>
    /// Generate a configuration to assert if the specified property has been notified.
    /// </summary>
    /// <param name="eventName">Name of the relevant event.</param>
    /// <param name="timeout">Maximum milliseconds to wait for events to occur.</param>
    /// <returns>Assert configuration for the specified event.</returns>
    public IAssertConfiguration Event(string eventName, int timeout)
    {
#if Universal81
      if (this.monitoredObject.GetType().GetRuntimeEvents().Any(o => string.Equals(o.Name, eventName, StringComparison.OrdinalIgnoreCase)))
#else
      if (this.monitoredObject.GetType().GetEvents(RelevantBindingFlags).Any(o => string.Equals(o.Name, eventName, StringComparison.OrdinalIgnoreCase)))
#endif
      {
        return new AssertConfiguration(this.recordedEventList, eventName, timeout);
      }
      else
      {
        throw new ArgumentException("The parameter must specify an event.");
      }
    }

#if Universal81
    private void Attach()
    {
      if (this.monitoredObject != null)
      {
        foreach (var eventInfo in this.monitoredObject.GetType().GetRuntimeEvents())
        {
          EventRecorder eventRecorder = new EventRecorder(eventInfo.Name, this.recordedEventList);
          Delegate handlerDelegate = eventRecorder.Handler.GetMethodInfo().CreateDelegate(eventInfo.EventHandlerType, eventRecorder);
          eventInfo.AddMethod.Invoke(this.monitoredObject, new object[] { handlerDelegate });
        }
      }
    }

    /// <summary>
    /// Stateful proxy that is bound to events to record when they are raised. 8.1 doesn't expose Delegate.Create used elsewhere.
    /// </summary>
    public class EventRecorder
    {
      private readonly string eventName;
      private readonly IList<INotification> recordedEventList;

      /// <summary>
      /// Initialises a new instance of the <see cref="EventRecorder"/> class.
      /// </summary>
      /// <param name="eventName">Name of the event to record with any event raised.</param>
      /// <param name="recordedEventList">Master list when raised events will be recorded to.</param>
      public EventRecorder(string eventName, IList<INotification> recordedEventList)
      {
        this.eventName = eventName;
        this.recordedEventList = recordedEventList;
      }

      /// <summary>
      /// Gets the handler to be bound to the event.
      /// </summary>
      public Action<object, object> Handler
      {
        get
        {
          return this.DynamicEventHandler;
        }
      }

      private void DynamicEventHandler(object sender, dynamic args)
      {
        lock (recordedEventListSynchLock)
        {
          this.recordedEventList.Add(new MonitoredEvent(this.eventName, (EventArgs)args));
        }
      }
    }
#else
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
        foreach (var eventInfo in this.monitoredObject.GetType().GetEvents(RelevantBindingFlags))
        {
          Action<object, EventArgs> handler = (s, e) =>
          {
            lock (recordedEventListSynchLock)
            {
              this.recordedEventList.Add(new MonitoredEvent(eventInfo.Name, e));
            }
          };
          Delegate convertedHandler = ConvertDelegate(handler, eventInfo.EventHandlerType);
          var addMethod = eventInfo.GetAddMethod(true);
          addMethod.Invoke(this.monitoredObject, new[] { convertedHandler });
        }
      }
    }
#endif
  }
}
