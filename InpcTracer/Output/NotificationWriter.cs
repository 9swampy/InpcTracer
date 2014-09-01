namespace InpcTracer.Output
{
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using InpcTracer.Tracing;

  internal class NotificationWriter
  {
    private const int MaxNumberOfNotificationsToWrite = 19;
    private readonly IEqualityComparer<INotification> notificationComparer;
    private readonly INotificationFormatter notificationFormatter;

    public NotificationWriter(INotificationFormatter notificationFormatter, IEqualityComparer<INotification> notificationComparer)
    {
      this.notificationFormatter = notificationFormatter;
      this.notificationComparer = notificationComparer;
    }

    public virtual void WriteNotifications(IEnumerable<INotification> notifications, IOutputWriter writer)
    {
      if (!notifications.Any())
      {
        return;
      }

      var notificationInfos = new List<NotificationInfo>();
      var notificationArray = notifications.ToArray();

      for (var i = 0; i < notificationArray.Length && i < MaxNumberOfNotificationsToWrite; i++)
      {
        var notification = notificationArray[i];

        if (i > 0 && this.notificationComparer.Equals(notificationInfos[notificationInfos.Count - 1].NotificationRaised, notification))
        {
          notificationInfos[notificationInfos.Count - 1].Repeat++;
        }
        else
        {
          notificationInfos.Add(new NotificationInfo
          {
            NotificationRaised = notification,
            NotificationNumber = i + 1,
            StringRepresentation = this.notificationFormatter.GetDescription(notification)
          });
        }
      }

      WriteNotifications(notificationInfos, writer);

      if (notificationArray.Length > MaxNumberOfNotificationsToWrite)
      {
        writer.WriteLine();
        writer.Write("... Found {0} more notifications not displayed here.", notificationArray.Length - MaxNumberOfNotificationsToWrite);
      }

      writer.WriteLine();
    }

    private static void WriteNotifications(IEnumerable<NotificationInfo> notificationInfos, IOutputWriter writer)
    {
      var lastNotification = notificationInfos.Last();
      var numberOfDigitsInLastNotificationNumber = lastNotification.NumberOfDigitsInNotificationNumber();

      foreach (var notificationInfo in notificationInfos)
      {
        if (notificationInfo.NotificationNumber > 1)
        {
          writer.WriteLine();
        }

        writer.Write(notificationInfo.NotificationNumber);
        writer.Write(": ");

        WriteSpaces(writer, numberOfDigitsInLastNotificationNumber - notificationInfo.NumberOfDigitsInNotificationNumber());

        using (writer.Indent())
        {
          writer.Write(notificationInfo.StringRepresentation);
        }

        if (notificationInfo.Repeat > 1)
        {
          writer.Write(" repeated ");
          writer.Write(notificationInfo.Repeat);
          writer.Write(" times");
          writer.WriteLine();
          writer.Write("...");
        }
      }
    }

    private static void WriteSpaces(IOutputWriter writer, int numberOfSpaces)
    {
      for (var i = 0; i < numberOfSpaces; i++)
      {
        writer.Write(" ");
      }
    }

    private class NotificationInfo
    {
      public NotificationInfo()
      {
        this.Repeat = 1;
      }

      public INotification NotificationRaised { get; set; }

      public int NotificationNumber { get; set; }

      public int Repeat { get; set; }

      public string StringRepresentation { get; set; }

      public int NumberOfDigitsInNotificationNumber()
      {
        return (int)Math.Log10(this.NotificationNumber);
      }
    }
  }
}