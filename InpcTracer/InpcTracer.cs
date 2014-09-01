namespace InpcTracer
{
  using System;
  using System.Collections.Generic;
  using System.ComponentModel;
  using System.Diagnostics.CodeAnalysis;
  using System.Linq.Expressions;
  using InpcTracer.Configuration;
  using InpcTracer.Framework;
  using InpcTracer.Tracing;

  public class InpcTracer<T>
    where T : INotifyPropertyChanged
  {
    private readonly INotifyPropertyChanged notifier;
    private readonly IExpressionValidator expressionValidator;
    private readonly IList<INotification> recordedEventList = new List<INotification>();

    public InpcTracer(INotifyPropertyChanged notifier) : this(notifier, new ExpressionValidator())
    {
    }

    public InpcTracer(INotifyPropertyChanged notifier, IExpressionValidator expressionValidator)
    {
      this.expressionValidator = expressionValidator;
      this.notifier = notifier;
      this.Attach();
      this.Clear();
    }

    ~InpcTracer()
    {
      this.Detach();
      this.Clear();
    }

    public InpcTracer<T> WhileProcessing(Action action)
    {
      Guard.AgainstNull(action, "action");

      this.Clear();
      action();
      return this;
    }

    [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "This is by design when using the Expression-, Action- and Func-types.")]
    public bool HasNotRecordedEvent<TResult>(Expression<Func<TResult>> expression)
    {
      var body = this.expressionValidator.ValidateAsMember<TResult>(expression);
      var result = new AssertConfiguration(this.recordedEventList, body);
      if (result.AtLeastOnce())
      {
        throw new ArgumentException(string.Format("Expected event '{0}' has been recorded", body.Member.Name));
      }

      return true;
    }

    [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "This is by design when using the Expression-, Action- and Func-types.")]
    public IAssertConfiguration RecordedEvent<TResult>(Expression<Func<TResult>> expression)
    {
      var body = this.expressionValidator.ValidateAsMember<TResult>(expression);
      var result = new AssertConfiguration(this.recordedEventList, body);
      return result;
    }

    [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "This is by design when using the Expression-, Action- and Func-types.")]
    public IOrderedAssertConfiguration FirstRecordedEvent<TResult>(Expression<Func<TResult>> expression)
    {
      MemberExpression body = this.expressionValidator.ValidateAsMember<TResult>(expression);
      OrderedAssertConfiguration result = new OrderedAssertConfiguration(this.recordedEventList, body, 0, this.expressionValidator);
      result.ThrowIfNotMatching();
      return result;
    }

    protected void Detach()
    {
      if (this.notifier != null)
      {
        this.notifier.PropertyChanged -= this.NotifierOnPropertyChanged;
      }
    }

    protected void Attach()
    {
      if (this.notifier != null)
      {
        this.notifier.PropertyChanged += this.NotifierOnPropertyChanged;
      }
    }

    protected void Clear()
    {
      this.recordedEventList.Clear();
    }

    private void NotifierOnPropertyChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
    {
      this.recordedEventList.Add(new Notification(propertyChangedEventArgs.PropertyName));
    }
  }
}