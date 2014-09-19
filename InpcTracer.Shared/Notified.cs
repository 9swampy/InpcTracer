namespace InpcTracer
{
  using System;
  using System.Diagnostics.CodeAnalysis;
  using System.Linq.Expressions;
  using Framework;
  using InpcTracer.Configuration;

  /// <summary>
  /// Provides syntax for specifying the number of times a notification must have been repeated.
  /// </summary>
  /// <example><code>A.CallTo(() => foo.Bar()).Assert(Happened.Once.Exactly);</code></example>
  [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1623:PropertySummaryDocumentationMustMatchAccessors", Justification = "Fluent API.")]
  public abstract class Raised
  {
    /// <summary>
    /// Asserts that a notification has not happened at all.
    /// </summary>
    public static Raised Never
    {
      get
      {
        return new NeverRepeated();
      }
    }

    /// <summary>
    /// The notification must have happened exactly the number of times that is specified in the next step.
    /// </summary>
    public static IRepeatSpecification Exactly
    {
      get
      {
        return new RepeatSpecification((actual, expected) => actual < expected ? RepeatMatch.UnsatisfiedPending : actual == expected ? RepeatMatch.SatisfiedPending : RepeatMatch.Unsatisfied, RepeatMatchType.Exactly);
      }
    }

    /// <summary>
    /// The notification must have happened any number of times greater than or equal to the number of times that is specified
    /// in the next step.
    /// </summary>
    public static IRepeatSpecification AtLeast
    {
      get
      {
        return new RepeatSpecification((actual, expected) => actual >= expected ? RepeatMatch.Satisfied : RepeatMatch.UnsatisfiedPending, RepeatMatchType.AtLeast);
      }
    }

    /// <summary>
    /// The notification must have happened any number of times less than or equal to the number of times that is specified
    /// in the next step.
    /// </summary>
    public static IRepeatSpecification NoMoreThan
    {
      get
      {
        return new RepeatSpecification((actual, expected) => actual <= expected ? RepeatMatch.SatisfiedPending : RepeatMatch.Unsatisfied, RepeatMatchType.NoMoreThan);
      }
    }

    /// <summary>
    /// Specifies that a notification must have been repeated a number of times
    /// that is validated by the specified repeatValidation argument.
    /// </summary>
    /// <param name="repeatValidation">A predicate that specifies the number of times
    /// a notification must have been made.</param>
    /// <returns>A Repeated-instance.</returns>
    [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "This is by design when using the Expression-, Action- and Func-types.")]
    public static Raised Like(Expression<Func<int, RepeatMatch>> repeatValidation)
    {
      return new ExpressionRepeated(repeatValidation);
    }

    /// <summary>
    /// When implemented gets a value indicating if the repeat is matched
    /// by the Happened-instance.
    /// </summary>
    /// <param name="repeat">The repeat of a notification.</param>
    /// <returns>True if the repeat is a match.</returns>
    internal abstract RepeatMatch Matches(int repeat);

    private class ExpressionRepeated
    : Raised
    {
      private readonly Expression<Func<int, RepeatMatch>> repeatValidation;

      public ExpressionRepeated(Expression<Func<int, RepeatMatch>> repeatValidation)
      {
        this.repeatValidation = repeatValidation;
      }

      public override string ToString()
      {
        return "the number of times specified by the predicate '{0}'".FormatInvariant(this.repeatValidation.ToString());
      }

      internal override RepeatMatch Matches(int repeat)
      {
        return this.repeatValidation.Compile().Invoke(repeat);
      }
    }

    private class RepeatSpecification : IRepeatSpecification
    {
      private readonly RepeatValidator repeatValidator;
      private readonly RepeatMatchType repeatMatchType;

      public RepeatSpecification(RepeatValidator repeatValidator, RepeatMatchType repeatMatchType)
      {
        this.repeatValidator = repeatValidator;
        this.repeatMatchType = repeatMatchType;
      }

      public delegate RepeatMatch RepeatValidator(int actualRepeat, int expectedRepeat);

      public Raised Once
      {
        get
        {
          return new RepeatedWithDescription(x => this.repeatValidator(x, 1), string.Format("{0} once", this.repeatMatchType.Description));
        }
      }

      public Raised Twice
      {
        get
        {
          return new RepeatedWithDescription(x => this.repeatValidator(x, 2), string.Format("{0} twice", this.repeatMatchType.Description));
        }
      }

      public Raised Times(int numberOfTimes)
      {
        return new RepeatedWithDescription(x => this.repeatValidator(x, numberOfTimes), "{0} {1} times".FormatInvariant(this.repeatMatchType.Description, numberOfTimes));
      }

      private class RepeatedWithDescription : Raised
      {
        private readonly Func<int, RepeatMatch> repeatValidator;
        private readonly string description;

        public RepeatedWithDescription(Func<int, RepeatMatch> repeatValidator, string description)
        {
          this.repeatValidator = repeatValidator;
          this.description = description;
        }

        public override string ToString()
        {
          return this.description;
        }

        internal override RepeatMatch Matches(int repeat)
        {
          return this.repeatValidator(repeat);
        }
      }
    }

    private class NeverRepeated : Raised
    {
      public override string ToString()
      {
        return "never";
      }

      internal override RepeatMatch Matches(int repeat)
      {
        return repeat == 0 ? RepeatMatch.SatisfiedPending : RepeatMatch.Unsatisfied;
      }
    }
  }
}