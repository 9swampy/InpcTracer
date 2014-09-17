namespace InpcTracer.Configuration
{
  using InpcTracer.Output;

  internal static class RepeatMetchExtensions
  {
    internal static TimeoutExtendedBoolResult ToTimeoutExtendedBoolResult(this RepeatMatch repeatMatch, int timeout)
    {
      switch (repeatMatch)
      {
        case RepeatMatch.Satisfied:
        case RepeatMatch.SatisfiedPending:
          return TimeoutExtendedBoolResult.True;
        case RepeatMatch.UnsatisfiedPending:
          if (timeout == 0)
          {
            return TimeoutExtendedBoolResult.False;
          }

          return TimeoutExtendedBoolResult.Timeout;
        case RepeatMatch.Unsatisfied:
        default:
          return TimeoutExtendedBoolResult.False;
      }
    }
  }
}