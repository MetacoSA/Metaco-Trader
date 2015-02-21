using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PowerWallet.ViewModel
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
            MessengerInstance.Register<AsyncCommand>(this, OnAsyncCommand);
        }

        void OnAsyncCommand(AsyncCommand command)
        {
            if (command.Task == null)
                command.Execute();
            if (command.Task == null)
            {
                Message = "Can't start the command";
                State = StatusState.Error;
                return;
            }

            command.Task.GetAwaiter().OnCompleted(
            () =>
            {
                UpdateState(command);
            });

            UpdateState(command);
        }

        private void UpdateState(AsyncCommand command)
        {
            if (new TaskStatus[]
             {
                 TaskStatus.Running,
                 TaskStatus.Created,
                 TaskStatus.WaitingForChildrenToComplete,
                 TaskStatus.WaitingToRun,
                 TaskStatus.WaitingForActivation,
             }.Contains(command.Task.Status))
            {
                Message = "";
                State = StatusState.Loading;
            }

            if (new TaskStatus[]
             {
                 TaskStatus.Faulted,
                 TaskStatus.Canceled,
             }.Contains(command.Task.Status))
            {
                State = StatusState.Error;
                Message = command.ErrorMessage;
                if (command.Task.Status == TaskStatus.Canceled)
                {
                    Message = "Canceled";
                }
            }

            if (new TaskStatus[]
             {
                 TaskStatus.RanToCompletion
             }.Contains(command.Task.Status))
            {
                if (command.ErrorMessage == "")
                    State = StatusState.Success;
                else
                {
                    Message = command.ErrorMessage;
                    State = StatusState.Error;
                }
            }
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
