namespace InpcTracer.Output
{
  using System;
  using System.Collections.Generic;
  using System.Diagnostics;
  using System.Linq;
  using System.Threading;
  using InpcTracer.Configuration;
  using InpcTracer.Framework;
  using InpcTracer.Tracing;

  internal class RecordedNotificationAsserter : IRecordedNotificationAsserter
  {
    private readonly NotificationWriter notificationWriter;
    private readonly IEnumerable<INotification> notifications;

    public RecordedNotificationAsserter(IEnumerable<INotification> notifications, NotificationWriter notificationWriter)
    {
      Guard.AgainstNull(notifications, "notifications");
      Guard.AgainstNull(notificationWriter, "notificationWriter");

      this.notifications = notifications;
      this.notificationWriter = notificationWriter;
    }

    public delegate IRecordedNotificationAsserter Factory(IEnumerable<INotification> calls);

    public virtual void AssertWasRecorded(Func<INotification, bool> notificationPredicate, string notificationDescription, Func<int, RepeatMatch> repeatPredicate, string repeatDescription)
    {
      this.AssertWasRecorded(notificationPredicate, notificationDescription, repeatPredicate, repeatDescription, 0);
    }

    public virtual void AssertWasRecorded(Func<INotification, bool> notificationPredicate, string notificationDescription, Func<int, RepeatMatch> repeatPredicate, string repeatDescription, int timeout)
    {
      if (this.WasRecorded(notificationPredicate, repeatPredicate, timeout) != TimeoutExtendedBoolResult.True)
      {
        var matchedNotificationCount = this.notifications.Count(notificationPredicate);
        var message = CreateExceptionMessage(this.notifications, this.notificationWriter, notificationDescription, repeatDescription, matchedNotificationCount);
        throw new ExpectationException(message);
      }
    }

    public bool WasRecorded(Func<INotification, bool> notificationPredicate, Func<int, RepeatMatch> repeatPredicate)
    {
      return this.WasRecorded(notificationPredicate, repeatPredicate, 0) == TimeoutExtendedBoolResult.True;
    }

    public TimeoutExtendedBoolResult WasRecorded(Func<INotification, bool> notificationPredicate, Func<int, RepeatMatch> repeatPredicate, int timeout)
    {
      TimeSpan delay = new TimeSpan(0, 0, 0, 0, 10);
      AwaitSuccessOrTimeout(this.HasDeterministicResult(notificationPredicate, repeatPredicate), new TimeSpan(0, 0, 0, 0, timeout), delay);
      var matchedNotificationCount = this.notifications.Count(notificationPredicate);
      return repeatPredicate(matchedNotificationCount).ToTimeoutExtendedBoolResult(timeout);
    }

    private static string CreateExceptionMessage(
      IEnumerable<INotification> notifications, NotificationWriter notificationWriter, string notificationDescription, string repeatDescription, int matchedNotificationCount)
    {
      var writer = new StringBuilderOutputWriter();
      writer.WriteLine();

      using (writer.Indent())
      {
        AppendNotificationDescription(notificationDescription, writer);
        AppendExpectation(notifications, repeatDescription, matchedNotificationCount, writer);
        AppendNotificationList(notifications, notificationWriter, writer);
        writer.WriteLine();
      }

      return writer.Builder.ToString();
    }

    private static void AppendNotificationDescription(string notificationDescription, IOutputWriter writer)
    {
      writer.WriteLine();
      writer.Write("Assertion failed for the following PropertyChanged notification:");
      writer.WriteLine();

      using (writer.Indent())
      {
        writer.Write(notificationDescription);
        writer.WriteLine();
      }
    }

    private static void AppendExpectation(IEnumerable<INotification> notifications, string repeatDescription, int matchedNotificationCount, IOutputWriter writer)
    {
      writer.Write("Expected to find it {0} ", repeatDescription);

      if (notifications.Any())
      {
        writer.Write("but found it #{0} times among the notifications:", matchedNotificationCount);
      }
      else
      {
        writer.Write("but no notifications were recorded.");
      }

      writer.WriteLine();
    }

    private static void AppendNotificationList(IEnumerable<INotification> notifications, NotificationWriter notificationWriter, IOutputWriter writer)
    {
      using (writer.Indent())
      {
        notificationWriter.WriteNotifications(notifications, writer);
      }
    }

    private static bool AwaitSuccessOrTimeout(Func<bool> task, TimeSpan timeout, TimeSpan pause)
    {
      if (pause.TotalMilliseconds < 0)
      {
        throw new ArgumentException("pause must be >= 0 milliseconds");
      }

      Stopwatch stopwatch = Stopwatch.StartNew();
      do
      {
        if (task())
        {
          return true;
        }

        Thread.Sleep((int)pause.TotalMilliseconds);
      }
      while (stopwatch.Elapsed < timeout);
      return false;
    }

    private Func<bool> HasDeterministicResult(Func<INotification, bool> notificationPredicate, Func<int, RepeatMatch> repeatPredicate)
    {
      return () => this.HasBeenRecorded(notificationPredicate, repeatPredicate) == RepeatMatch.Satisfied &&
                   this.HasBeenRecorded(notificationPredicate, repeatPredicate) == RepeatMatch.Unsatisfied;
    }

    private RepeatMatch HasBeenRecorded(Func<INotification, bool> notificationPredicate, Func<int, RepeatMatch> repeatPredicate)
    {
      var matchedNotificationCount = this.notifications.Count(notificationPredicate);
      System.Diagnostics.Debug.Print(string.Format("Notifications.Count={0}", matchedNotificationCount));
      return repeatPredicate(matchedNotificationCount);
    }
  }
}