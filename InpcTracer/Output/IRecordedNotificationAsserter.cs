namespace InpcTracer.Output
{
  using System;
  using InpcTracer.Configuration;
  using InpcTracer.Tracing;

  internal interface IRecordedNotificationAsserter
  {
    void AssertWasRecorded(Func<INotification, bool> memberPredicate, string memberDescription, Func<int, RepeatMatch> repeatPredicate, string repeatDescription);

    bool WasRecorded(Func<INotification, bool> notificationPredicate, Func<int, RepeatMatch> repeatPredicate);
  }
}