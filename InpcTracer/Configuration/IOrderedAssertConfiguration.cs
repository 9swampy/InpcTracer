namespace InpcTracer.Configuration
{
  using System;
  using System.Diagnostics.CodeAnalysis;
  using System.Linq.Expressions;

  public interface IOrderedAssertConfiguration : IAssertConfiguration
  {
    [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "This is by design when using the Expression-, Action- and Func-types.")]
    IOrderedAssertConfiguration ThenRecordedEvent<T>(Expression<Func<T>> expression);
  }
}