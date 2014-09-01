namespace InpcTracer.NTests
{
  using FakeItEasy;
  using System.Collections;
  using System.Linq;

  public static class CustomArgumentConstraints
  {
    public static T IsThisSequence<T>(this IArgumentConstraintManager<T> scope, T collection) where T : IEnumerable
    {
      return scope.Matches(
          x => x.Cast<object>().SequenceEqual(collection.Cast<object>()),
          "This sequence: " + collection.Cast<object>().ToCollectionString(x => x.ToString(), ", "));
    }
  }
}