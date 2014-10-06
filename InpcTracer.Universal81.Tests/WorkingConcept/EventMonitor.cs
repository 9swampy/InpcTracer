namespace InpcTracer
{
  using System;
  using System.Collections.Generic;
  using System.ComponentModel;
  using System.Linq;
  using System.Linq.Expressions;
  using System.Reflection;
  //using InpcTracer.Configuration;
  using InpcTracer.Tracing;

  /// <summary>
  /// Records events raised by the monitored object.
  /// </summary>
  /// <typeparam name="T">Type of the monitored object.</typeparam>
  public class EventMonitor<T>
  {
    //private const BindingFlags RelevantBindingFlags = BindingFlags.FlattenHierarchy | BindingFlags.IgnoreCase | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
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

    public class EventRecorder
    {
      private readonly string eventName;
      private readonly IList<INotification> recordedEventList;

      public EventRecorder(string eventName, IList<INotification> recordedEventList)
      {
        this.eventName = eventName;
        this.recordedEventList = recordedEventList;
      }

      public Action<object, object> Handler
      {
        get
        {
          return OnBackKeyPressed;
        }
      }

      private void OnBackKeyPressed(object sender, dynamic args)
      {
        lock (recordedEventListSynchLock)
        {
          this.recordedEventList.Add(new MonitoredEvent(this.eventName, (EventArgs)args));
        }
      }
    }
  }
}
