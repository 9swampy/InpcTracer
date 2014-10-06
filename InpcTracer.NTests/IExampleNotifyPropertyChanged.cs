namespace InpcTracer.NTests
{
  using System.ComponentModel;

  public interface IExampleNotifyPropertyChanged : INotifyPropertyChanged
  {
    object PropertyA { get; set; }

    object PropertyB { get; set; }
  }
}