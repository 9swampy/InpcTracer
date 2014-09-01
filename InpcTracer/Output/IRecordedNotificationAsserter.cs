namespace InpcTracer.Output
{
  using System;
  using InpcTracer.Tracing;

  internal interface IRecordedNotificationAsserter
  {
    void AssertWasRecorded(Func<INotification, bool> memberPredicate, string memberDescription, Func<int, bool> repeatPredicate, string repeatDescription);

    bool WasRecorded(Func<INotification, bool> notificationPredicate, Func<int, bool> repeatPredicate);
  }
}