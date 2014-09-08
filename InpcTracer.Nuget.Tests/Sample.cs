namespace InpcTracer.Nuget.Tests
{
  using System;
  using Microsoft.VisualStudio.TestTools.UnitTesting;
  using FakeItEasy;
  using System.ComponentModel;
  using InpcTracer.Configuration;
  
  [TestClass]
  public class Sample
  {
    public interface ITraceable : INotifyPropertyChanged
    {
      bool Active { get; set; }
    }

    [TestMethod]
    public void TestNotifyPropertyChangedFired()
    {
      ITraceable target = A.Fake<ITraceable>();
      A.CallTo(target).Where(x => x.Method.Name == "set_Active")
                .Invokes(() =>
                {
                  target.PropertyChanged += Raise.With(new PropertyChangedEventArgs("Active")).Now;
                });

      var tracer = new InpcTracer.InpcTracer<ITraceable>(target);

      // Check for one event
      Assert.IsTrue(tracer.WhileProcessing(() => target.Active = true).PropertyChanged(() => target.Active).ExactlyOnce());
      // or
      tracer.WhileProcessing(() => target.Active = true).PropertyChanged(() => target.Active).MustHaveBeen(Notified.Exactly.Once);

      // Check for exact order of two events
      tracer.WhileProcessing(() =>
      {
        target.Active = false;
        target.Active = true;
      }).FirstPropertyChanged(() => target.Active).ThenPropertyChanged(() => target.Active);
    }
  }
}
