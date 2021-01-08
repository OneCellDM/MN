using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;

namespace MN.Model
{
    public class NoteModel:INotifyPropertyChanged
    {
        public bool IsNew = false;
        private string _Title="";
        private string _Data="";
        private DateTime _DateTime;
        public int Id { get; set; }
        public string Title { get=> _Title; set {
                _Title = value;
                OnPropertyChanged();
            } 
        }
        public string Data { get=> _Data;  set {

                _Data = value;
                OnPropertyChanged();
            
            } 
        }
        public DateTime DateTime {get => _DateTime; set{
                _DateTime = value;
                OnPropertyChanged();
            }
        
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }
    }
}
