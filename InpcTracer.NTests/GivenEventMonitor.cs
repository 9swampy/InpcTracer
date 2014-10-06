namespace InpcTracer.NTests
{
  using System;
  using System.Collections.ObjectModel;
  using FluentAssertions;
  using NUnit.Framework;

  [TestFixture]
  public class GivenEventMonitor
  {
    private static ObservableCollection<int> observableTarget;
    private static ReadOnlyObservableCollection<int> target;
        
    public static void OuterClassInitialize()
    {
      observableTarget = new ObservableCollection<int>();
      target = new ReadOnlyObservableCollection<int>(observableTarget);
    }

    [TestFixture]
    public class OfObservableCollection
    {
      [TestFixtureSetUp]
      public void ClassInitialize()
      {
        OuterClassInitialize();
      }

      [Test]
      public void Test()
      {
        EventMonitor<ObservableCollection<int>> sut = new EventMonitor<ObservableCollection<int>>(observableTarget);
        observableTarget.Add(1);
        sut.Event("CollectionChanged").MustHaveBeen(Raised.Exactly.Once);
      }
    }

    [TestFixture]
    public class OfReadOnlyObservableCollectionDecoratedClass
    {
      private static EventMonitor<ReadOnlyObservableCollection<int>> sut;

      [TestFixtureSetUp]
      public void ClassInitialize()
      {
        OuterClassInitialize();
        sut = new EventMonitor<ReadOnlyObservableCollection<int>>(target);
      }

      [Test]
      public void HiddenEventsShouldBeMonitored()
      {
        observableTarget.Add(1);
        sut.Event("CollectionChanged").MustHaveBeen(Raised.Exactly.Once);
      }

      [Test]
      public void DerivedEventsShouldBeRecognised()
      {
        Action act = () => sut.Event("CollectionChanged");

        act.ShouldNotThrow<System.ArgumentException>();
      }

      [Test]
      public void EventsShouldBeRecognisedRegardlessOfCase()
      {
        Action act = () => sut.Event("collectionChanged");

        act.ShouldNotThrow<System.ArgumentException>();
      }
    }
  }
}
