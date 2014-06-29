using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace BTControl
{
  public class BTContent : INotifyPropertyChanged
  {
    public event PropertyChangedEventHandler PropertyChanged;

    private string data;
    public string Data 
    { 
      get { return data; }
      set
      {
        if (value != data)
        {
          data = value;
          OnPropertyChanged("Data");
        }
      }
    }

    protected void OnPropertyChanged(string PropertyName)
    {
      PropertyChangedEventHandler handler = PropertyChanged;
      if (handler != null)
      {
        handler(this, new PropertyChangedEventArgs(PropertyName));
      }
    }
  }
}
