namespace InpcTracer.Nuget.Tests
{
  using System;
  using Microsoft.VisualStudio.TestTools.UnitTesting;
  using FakeItEasy;
  using System.ComponentModel;
  using Repeated = InpcTracer.Configuration.Repeated;

  [TestClass]
  public class Sample
  {
    public interface ITraceable : INotifyPropertyChanged
    {
      bool Active { get; set; }
    }

    [TestMethod]
    public void Test_Notify_Property_Changed_Fired()
    {
      ITraceable target = A.Fake<ITraceable>();
      A.CallTo(target).Where(x => x.Method.Name == "set_Active")
                .Invokes(() =>
                {
                  target.PropertyChanged += Raise.With(new PropertyChangedEventArgs("Active")).Now;
                });

      var tracer = new InpcTracer.InpcTracer<ITraceable>(target);

      // Check for one event
      Assert.IsTrue(tracer.WhileProcessing(() => target.Active = true).RecordedEvent(() => target.Active).ExactlyOnce());
      // or
      tracer.WhileProcessing(() => target.Active = true).RecordedEvent(() => target.Active).MustHaveHappened(Repeated.Exactly.Once);

      // Check for exact order of two events
      tracer.WhileProcessing(() =>
      {
        target.Active = false;
        target.Active = true;
      }).FirstRecordedEvent(() => target.Active).ThenRecordedEvent(() => target.Active);
    }
  }
}
