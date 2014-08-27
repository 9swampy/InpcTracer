namespace InpcTracer.Tests
{
  using System;
  using System.ComponentModel;
  using System.Linq;

  internal class Project : INotifyPropertyChanged
  {
    private bool active;

    private string path;

    public event PropertyChangedEventHandler PropertyChanged;

    public bool Active
    {
      get
      {
        return this.active;
      }
      set
      {
        if (this.active != value)
        {
          this.active = value;
          this.OnPropertyChanged("Active");
        }
      }
    }

    public string Path
    {
      get
      {
        return this.path;
      }
      set
      {
        if (this.path != value)
        {
          this.path = value;
          this.OnPropertyChanged("Path");
          this.Active = !this.Active;
        }
      }
    }
    
    private void OnPropertyChanged(string propertyName)
    {
      if (this.PropertyChanged != null)
      {
        this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
      }
    }
  }
}