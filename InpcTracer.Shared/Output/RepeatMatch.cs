namespace InpcTracer.Configuration
{
  /// <summary>
  /// Indicates the degree to which a repeat specification has been satisfied.
  /// </summary>
  public enum RepeatMatch
  {
    /// <summary>
    /// Indicates that the repeat specification has been precisely satisfied, regardless of how long the code waits.
    /// </summary>
    Satisfied,

    /// <summary>
    /// Indicates that the repeat specification has been satisfied, but if on a timeout and we had waited longer the result could have changed.
    /// </summary>
    SatisfiedPending,

    /// <summary>
    /// Indicates that the repeat specification has not been satisfied, but if on a timeout and we had waited longer the result could have changed.
    /// </summary>
    UnsatisfiedPending,

    /// <summary>
    /// Indicates that the repeat specification has been violated, regardless of how long the code waits.
    /// </summary>
    Unsatisfied
  }
}