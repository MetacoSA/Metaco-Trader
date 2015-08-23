using Metaco.Trader.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Metaco.Trader.ViewModel
{
    public enum StatusState
    {
        Loading,
        Error,
        Success
    }
    public class StatusMainViewModel : PWViewModelBase
    {
        public StatusMainViewModel()
        {
            MessengerInstance.Register<StatusMessage>(this, OnStatusCommand);
        }

        void OnStatusCommand(StatusMessage message)
        {
            State = message.Status;
            if (message.Message != null)
                Message = message.Message;
        }

        
        private StatusState? _State;
        public StatusState? State
        {
            get
            {
                return _State;
            }
            set
            {
                if (value != _State)
                {
                    _State = value;
                    OnPropertyChanged(() => this.State);
                }
            }
        }

        private string _Message;
        public string Message
        {
            get
            {
                return _Message;
            }
            set
            {
                if (value != _Message)
                {
                    _Message = value;
                    OnPropertyChanged(() => this.Message);
                }
            }
        }
    }
}
