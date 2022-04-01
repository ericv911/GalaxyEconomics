using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace SpaceTrader
{
    public class MessageViewModel : BaseViewModel, INotifyPropertyChanged
    {
        protected string _messagestring;
        public string MessageString
        {
            get { return _messagestring; }
            set 
            {
                _messagestring = value;
                OnPropertyChanged();
            }
        }
        public MessageViewModel(Window window)
        {
            /* window events */
            window.Loaded += (sender, e) =>
            {
                MessageString = "placeholder"; 
                EventSystem.Subscribe<TickerSymbolSelectedMessage>(SetMessageString);
            };

            window.Closed += (sender, e) =>
            {
            };
        }
        private void SetMessageString(TickerSymbolSelectedMessage msg)
        {
            MessageString = msg.MessageString;
        }
    }
}


