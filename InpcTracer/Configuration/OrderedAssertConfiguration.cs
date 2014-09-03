namespace InpcTracer.Configuration
{
  using System;
  using System.Collections.Generic;
  using System.Linq.Expressions;
  using InpcTracer.Tracing;

  /// <summary>
  /// Allows the developer to assert on an ordered chain of notifications configured.
  /// </summary>
  public class OrderedAssertConfiguration : AssertConfiguration, IOrderedAssertConfiguration
  {
    private readonly IList<INotification> recordedNotifications;
    private readonly MemberExpression memberExpression;
    private readonly int index;
    private readonly IExpressionValidator expressionValidator;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="recordedNotifications">Collection of notifications recorded.</param>
    /// <param name="memberExpression">The MemberExpression that produces the relevant property.</param>
    /// <param name="index"></param>
    /// <param name="expressionValidator"></param>
    public OrderedAssertConfiguration(IList<INotification> recordedNotifications, MemberExpression memberExpression, int index, IExpressionValidator expressionValidator) : base(recordedNotifications, memberExpression)
    {
      this.expressionValidator = expressionValidator;
      this.memberExpression = memberExpression;
      this.index = index;
      this.recordedNotifications = recordedNotifications;
    }

    /// <summary>
    /// Assert that the next notification in the chain matches the specified property.
    /// </summary>
    /// <typeparam name="TResult">Type of the relevant property</typeparam>
    /// <param name="expression">A function that produces the relevant property</param>
    /// <returns>The next IOrderedAssertConfiguration in the notification chain</returns>
    public IOrderedAssertConfiguration ThenRecordedEvent<TResult>(Expression<Func<TResult>> expression)
    {
      var validatedExpression = this.expressionValidator.ValidateAsMember<TResult>(expression);
      OrderedAssertConfiguration result = new OrderedAssertConfiguration(this.recordedNotifications, validatedExpression, this.index + 1, this.expressionValidator);
      result.ThrowIfNotMatching();
      return result;
    }

    internal void ThrowIfNotMatching()
    {
      if (this.recordedNotifications.Count <= this.index)
      {
        throw new ArgumentException(string.Format("Expected event '{0}' but no more events left", this.memberExpression.Member.Name));
      }

      if (this.recordedNotifications[this.index].PropertyName != this.memberExpression.Member.Name)
      {
        throw new ArgumentException(string.Format("Event {0} was not at position {1}", this.memberExpression.Member.Name, this.index));
      }
    }
  }
}