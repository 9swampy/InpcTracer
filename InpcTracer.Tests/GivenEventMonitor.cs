namespace InpcTracer.Tests
{
  using System;
  using System.Collections.ObjectModel;
  using FluentAssertions;
  using InpcTracer.Configuration;
  using Microsoft.VisualStudio.TestTools.UnitTesting;

  public class GivenEventMonitor
  {
    private static ObservableCollection<int> observableTarget;
    private static ReadOnlyObservableCollection<int> target;
        
    public static void OuterClassInitialize(TestContext context)
    {
      observableTarget = new ObservableCollection<int>();
      target = new ReadOnlyObservableCollection<int>(observableTarget);
    }

    [TestClass]
    public class OfObservableCollection
    {
      [ClassInitialize]
      public static void ClassInitialize(TestContext context)
      {
        OuterClassInitialize(context);
      }

      [TestMethod]
      public void Test()
      {
        EventMonitor<ObservableCollection<int>> sut = new EventMonitor<ObservableCollection<int>>(observableTarget);
        observableTarget.Add(1);
        sut.Event("CollectionChanged").MustHaveBeen(Raised.Exactly.Once);
      }
    }

    [TestClass]
    public class OfReadOnlyObservableCollectionDecoratedClass
    {
      private static EventMonitor<ReadOnlyObservableCollection<int>> sut;

      [ClassInitialize]
      public static void ClassInitialize(TestContext context)
      {
        OuterClassInitialize(context);
        sut = new EventMonitor<ReadOnlyObservableCollection<int>>(target);
      }

      [TestMethod]
      public void HiddenEventsShouldBeMonitored()
      {
        observableTarget.Add(1);
        sut.Event("CollectionChanged").MustHaveBeen(Raised.Exactly.Once);
      }

      [TestMethod]
      public void DerivedEventsShouldBeRecognised()
      {
        Action act = () => sut.Event("CollectionChanged");

        act.ShouldNotThrow<System.ArgumentException>();
      }

      [TestMethod]
      public void EventsShouldBeRecognisedRegardlessOfCase()
      {
        Action act = () => sut.Event("collectionChanged");

        act.ShouldNotThrow<System.ArgumentException>();
      }
    }
  }
}
