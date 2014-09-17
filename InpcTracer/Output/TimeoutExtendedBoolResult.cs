namespace InpcTracer.Output
{
  /// <summary>
  /// Extends a boolean result to include a timeout condition.
  /// </summary>
  public enum TimeoutExtendedBoolResult
  {
    /// <summary>
    /// Indicates a true result was obtained.
    /// </summary>
    True,

    /// <summary>
    /// Indicates a false result was obtained.
    /// </summary>
    False,

    /// <summary>
    /// Indicates that a timeout occurred while waiting for a definite outcome.
    /// </summary>
    Timeout
  }
}