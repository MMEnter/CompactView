using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;

namespace CompactView.Data
{
    public class Website : INotifyPropertyChanged
    {
        private long _iD;
        private string _name;
        private Uri _uRL;
        private Symbol _symbol;
        private char _symbolAsChar;

        public long ID
        {
            get { return _iD; }
            set
            {
                _iD = value;
                RaisePropertyChanged("ID");
            }
        }
        public string Name
        {
            get { return _name; }
            set
            {
                _name = value;
                RaisePropertyChanged("Name");
            }
        }
        public Uri URL
        {
            get { return _uRL; }
            set
            {
                _uRL = value;
                RaisePropertyChanged("URL");
            }
        }
        public Symbol Symbol
        {
            get { return _symbol; }
            set
            {
                _symbol = value;
                RaisePropertyChanged("Symbol");
            }
        }
        public char SymbolAsChar
        {
            get { return (char)Symbol; }
            set
            {
                _symbolAsChar = value;
                RaisePropertyChanged("SymbolAsChar");
            }
        }
        public event PropertyChangedEventHandler PropertyChanged;
        protected void RaisePropertyChanged(string name)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(name));
            }
        }
    }
}
