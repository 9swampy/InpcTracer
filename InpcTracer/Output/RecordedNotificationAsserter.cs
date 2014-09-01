namespace InpcTracer.Output
{
  using System;
  using System.Collections.Generic;
  using System.Linq;
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

    public virtual void AssertWasRecorded(Func<INotification, bool> notificationPredicate, string notificationDescription, Func<int, bool> repeatPredicate, string repeatDescription)
    {
      var matchedNotificationCount = this.notifications.Count(notificationPredicate);
      if (!repeatPredicate(matchedNotificationCount))
      {
        var message = CreateExceptionMessage(this.notifications, this.notificationWriter, notificationDescription, repeatDescription, matchedNotificationCount);
        throw new ExpectationException(message);
      }
    }

    public bool WasRecorded(Func<INotification, bool> notificationPredicate, Func<int, bool> repeatPredicate)
    {
      var matchedNotificationCount = this.notifications.Count(notificationPredicate);
      if (repeatPredicate(matchedNotificationCount))
      {
        return true;
      }

      return false;
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
  }
}