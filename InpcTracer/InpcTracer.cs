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

  /// <summary>
  /// Wrapper the facilitates trace recording of INotifyPropertyChanged events.
  /// </summary>
  /// <typeparam name="TNotifyPropertyChanged">Type wrapped.</typeparam>
  public class InpcTracer<TNotifyPropertyChanged>
    where TNotifyPropertyChanged : INotifyPropertyChanged
  {
    private readonly TNotifyPropertyChanged notifier;
    private readonly IExpressionValidator expressionValidator;
    private readonly IList<INotification> recordedEventList = new List<INotification>();

    /// <summary>
    /// Creates a wrapper around the specified notifier.
    /// </summary>
    /// <param name="notifier"></param>
    public InpcTracer(TNotifyPropertyChanged notifier)
      : this(notifier, new ExpressionValidator())
    {
    }

    /// <summary>
    /// Creates a wrapper around the specified notifier.
    /// </summary>
    /// <param name="notifier"></param>
    /// <param name="expressionValidator"></param>
    public InpcTracer(TNotifyPropertyChanged notifier, IExpressionValidator expressionValidator)
    {
      this.expressionValidator = expressionValidator;
      this.notifier = notifier;
      this.Attach();
      this.Clear();
    }

    /// <summary>
    /// Destructor to clean up references held by the instance.
    /// </summary>
    ~InpcTracer()
    {
      this.Detach();
      this.Clear();
    }

    /// <summary>
    /// Records notifications after clearing any pre-existing notifications.
    /// </summary>
    /// <param name="action">Action to process.</param>
    /// <returns></returns>
    public InpcTracer<TNotifyPropertyChanged> WhileProcessing(Action action)
    {
      Guard.AgainstNull(action, "action");

      this.Clear();
      action();
      return this;
    }

    /// <summary>
    /// Generate a configuration to assert if the specified property has been notified. 
    /// </summary>
    /// <typeparam name="TResult">Type of the relevant property.</typeparam>
    /// <param name="expression">A function that produces the relevant property.</param>
    /// <returns>A configuration element to determine the assertion.</returns>
    [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "This is by design when using the Expression-, Action- and Func-types.")]
    public IAssertConfiguration RecordedEvent<TResult>(Expression<Func<TResult>> expression)
    {
      var body = this.expressionValidator.ValidateAsMember<TResult>(expression);
      var result = new AssertConfiguration(this.recordedEventList, body);
      return result;
    }

    /// <summary>
    /// Assert that the first notification in the chain matches the specified property.
    /// </summary>
    /// <typeparam name="TResult">Type of the relevant property.</typeparam>
    /// <param name="expression">A function that produces the relevant property.</param>
    /// <returns>The next IOrderedAssertConfiguration in the notification chain.</returns>
    [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "This is by design when using the Expression-, Action- and Func-types.")]
    public IOrderedAssertConfiguration FirstRecordedEvent<TResult>(Expression<Func<TResult>> expression)
    {
      MemberExpression body = this.expressionValidator.ValidateAsMember<TResult>(expression);
      OrderedAssertConfiguration result = new OrderedAssertConfiguration(this.recordedEventList, body, 0, this.expressionValidator);
      result.ThrowIfNotMatching();
      return result;
    }

    private void Detach()
    {
      if (this.notifier != null)
      {
        this.notifier.PropertyChanged -= this.NotifierOnPropertyChanged;
      }
    }

    private void Attach()
    {
      if (this.notifier != null)
      {
        this.notifier.PropertyChanged += this.NotifierOnPropertyChanged;
      }
    }

    private void Clear()
    {
      this.recordedEventList.Clear();
    }

    private void NotifierOnPropertyChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
    {
      this.recordedEventList.Add(new Notification(propertyChangedEventArgs.PropertyName));
    }
  }
}