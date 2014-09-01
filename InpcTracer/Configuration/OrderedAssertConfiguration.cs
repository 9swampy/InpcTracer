namespace InpcTracer.Configuration
{
  using System;
  using System.Collections.Generic;
  using System.Linq.Expressions;
  using InpcTracer.Tracing;

  public class OrderedAssertConfiguration : AssertConfiguration, IOrderedAssertConfiguration
  {
    private readonly IList<INotification> recordedNotifications;
    private readonly MemberExpression memberExpression;
    private readonly int index;
    private readonly IExpressionValidator expressionValidator;

    public OrderedAssertConfiguration(IList<INotification> recordedNotifications, MemberExpression memberExpression, int index, IExpressionValidator expressionValidator) : base(recordedNotifications, memberExpression)
    {
      this.expressionValidator = expressionValidator;
      this.memberExpression = memberExpression;
      this.index = index;
      this.recordedNotifications = recordedNotifications;
    }

    public IOrderedAssertConfiguration ThenRecordedEvent<T>(Expression<Func<T>> expression)
    {
      var validatedExpression = this.expressionValidator.ValidateAsMember<T>(expression);
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