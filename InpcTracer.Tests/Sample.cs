namespace InpcTracer.Tests
{
  using Microsoft.VisualStudio.TestTools.UnitTesting;

  [TestClass]
  public class Sample
  {
    [TestMethod]
    public void Test_Notify_Property_Changed_Fired()
    {
      var p = new Project();
      var tracer = new InpcTracer.InpcTracer<Project>(p);

      // Check for one event
      //Assert.IsTrue(tracer.WhileProcessing(() => p.Active = true).HasRecordedEvent(() => p.Active).MustHaveHappened(InpcTracer.Configuration.Repeated.Exactly.Once));
      tracer.WhileProcessing(() => p.Active = true).RecordedEvent(() => p.Active).MustHaveHappened(InpcTracer.Configuration.Repeated.Exactly.Once);

      // Check for exact order of two events
      tracer.WhileProcessing(() => p.Path = "test").FirstRecordedEvent(() => p.Path).ThenRecordedEvent(() => p.Active);
    }
  }
}