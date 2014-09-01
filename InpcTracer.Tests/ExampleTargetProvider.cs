namespace InpcTracer.Tests
{
  using FakeItEasy;
  using System.ComponentModel;

  public class ExampleTargetProvider
  {
    public static IExampleNotifyPropertyChanged ACorrectlyWiredTarget()
    {
      IExampleNotifyPropertyChanged target = FakeItEasy.A.Fake<IExampleNotifyPropertyChanged>();
      FakeItEasy.A
                .CallTo(target)
                .Where(x => x.Method.Name == "set_PropertyA")
                .Invokes(() =>
                {
                  target.PropertyChanged += Raise.With(new PropertyChangedEventArgs("PropertyA")).Now;
                });
      FakeItEasy.A
                .CallTo(target)
                .Where(x => x.Method.Name == "set_PropertyB")
                .Invokes(() =>
                {
                  target.PropertyChanged += Raise.With(new PropertyChangedEventArgs("PropertyB")).Now;
                });
      return target;
    }
  }
}