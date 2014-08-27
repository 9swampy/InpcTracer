namespace InpcTracer
{
  using System;
  using System.Collections.Generic;
  using System.ComponentModel;
  using System.Linq;
  using System.Linq.Expressions;

  public class InpcTracer<T>
    where T : INotifyPropertyChanged
  {
    protected INotifyPropertyChanged notifier;
    protected IList<string> recordedEventList = new List<string>();

    public InpcTracer(INotifyPropertyChanged notifier)
    {
      this.notifier = notifier;
      this.Attach();
      this.Clear();
    }

    ~InpcTracer()
    {
      this.Detach();
      this.Clear();
    }

    public InpcTracer<T> WhileProcessing(Action a)
    {
      this.Clear();
      a();
      return this;
    }

    public InpcTracerRecordedEvent HasRecordedEvent<T>(Expression<Func<T>> expression)
    {
      var body = ValidateExpressionAsMember<T>(expression);
      return new InpcTracerRecordedEvent(this, body);
    }

    public InpcTracerRecordedEventReader FirstRecordedEvent<T>(Expression<Func<T>> expression)
    {
      this.ThrowIfNextNotMatching(expression, 0);
      return new InpcTracerRecordedEventReader(this, 1);
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

    private static MemberExpression ValidateExpressionAsMember<T>(Expression<Func<T>> expression)
    {
      if (expression == null)
      {
        throw new ArgumentNullException("expression");
      }
      var body = expression.Body as MemberExpression;
      if (body == null)
      {
        throw new ArgumentException("The body must be a member expression");
      }
      return body;
    }

    private void ThrowIfNextNotMatching<T>(Expression<Func<T>> expression, int eventPos)
    {
      var body = ValidateExpressionAsMember<T>(expression);
      if (this.recordedEventList.Count <= eventPos)
      {
        throw new ArgumentException(string.Format("Expected event '{0}' but no more events left", body.Member.Name));
      }
      if (this.recordedEventList[eventPos] != body.Member.Name)
      {
        throw new ArgumentException(string.Format("Event {0} was not at position {1}", body.Member.Name, eventPos));
      }
    }

    private void NotifierOnPropertyChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
    {
      this.recordedEventList.Add(propertyChangedEventArgs.PropertyName);
    }

    public class InpcTracerRecordedEventReader
    {
      private readonly InpcTracer<T> inpcTracer;

      private readonly int eventPos;

      public InpcTracerRecordedEventReader(InpcTracer<T> inpcTracer, int eventPos)
      {
        this.eventPos = eventPos;
        this.inpcTracer = inpcTracer;
      }

      public InpcTracerRecordedEventReader ThenRecordedEvent<T>(Expression<Func<T>> expression)
      {
        this.inpcTracer.ThrowIfNextNotMatching(expression, this.eventPos);
        return new InpcTracerRecordedEventReader(this.inpcTracer, this.eventPos + 1);
      }
    }

    public class InpcTracerRecordedEvent
    {
      private readonly InpcTracer<T> inpcTracer;
      private readonly MemberExpression memberExpression;

      public InpcTracerRecordedEvent(InpcTracer<T> inpcTracer, MemberExpression memberExpression)
      {
        this.inpcTracer = inpcTracer;
        this.memberExpression = memberExpression;
      }

      public bool ExactlyOnce()
      {
        return this.inpcTracer.recordedEventList.Count(o => o == this.memberExpression.Member.Name) == 1;
      }

      public bool AtLeastOnce()
      {
        return this.inpcTracer.recordedEventList.Contains(this.memberExpression.Member.Name);
      }
    }
  }
}