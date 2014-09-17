namespace InpcTracer.Configuration
{
  /// <summary>
  /// Allows the developer to configure how often an assert notification should be repeated.
  /// </summary>
  public class RepeatMatchType
  {
    private RepeatMatchType(string description)
    {
      this.Description = description;
    }
    
    /// <summary>
    /// Gets a condition requiring a precise match.
    /// </summary>
    public static RepeatMatchType Exactly
    {
      get
      {
        return new RepeatMatchType("exactly");
      }
    }

    /// <summary>
    /// Gets a condition requiring that the match must have occurred a minimum number of times.
    /// </summary>
    public static RepeatMatchType AtLeast
    {
      get
      {
        return new RepeatMatchType("at least");
      }
    }

    /// <summary>
    /// Gets a condition requiring that the match must have occurred less than a maximum number of times.
    /// </summary>
    public static RepeatMatchType NoMoreThan
    {
      get
      {
        return new RepeatMatchType("no more than");
      }
    }

    /// <summary>
    /// Gets the description to append to related output messages.
    /// </summary>
    public string Description { get; private set; }
  }
}