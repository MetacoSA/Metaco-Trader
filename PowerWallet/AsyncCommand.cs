using GalaSoft.MvvmLight.Messaging;
using PowerWallet.Messages;
using PowerWallet.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;

namespace PowerWallet
{
    public class AsyncCommand : PWViewModelBase, ICommand
    {
        Func<AsyncCommand, Task> _Exec;
        public AsyncCommand(Func<AsyncCommand, Task> exec)
        {
            if (exec == null)
                throw new ArgumentNullException("exec");
            _Exec = exec;
        }
        public AsyncCommand()
        {

        }
        Task _Task;
        internal Task Task
        {
            get
            {
                return _Task;
            }
            set
            {
                _Task = value;
                OnCanExecuteChanged();
            }
        }

        public bool IsExecuting
        {
            get
            {
                return _Task != null &&
                    new TaskStatus[] { 
                        TaskStatus.Created, 
                        TaskStatus.Running, 
                        TaskStatus.WaitingForActivation, 
                        TaskStatus.WaitingForChildrenToComplete, 
                        TaskStatus.WaitingToRun }.Contains(_Task.Status);
            }
        }

        public bool IsRunnable
        {
            get
            {
                return CanExecute(null);
            }
        }

        #region ICommand Members

        public bool CanExecute(object parameter)
        {
            return !IsExecuting;
        }

        protected void OnCanExecuteChanged()
        {
            if (CanExecuteChanged != null)
                CanExecuteChanged(this, EventArgs.Empty);
            OnPropertyChanged(() => this.IsRunnable);
        }
        public event EventHandler CanExecuteChanged;

        public void Execute()
        {
            Execute(null);
        }


        private string _Status;
        public string Status
        {
            get
            {
                return _Status;
            }
            private set
            {
                if (value != _Status)
                {
                    _Status = value;
                    OnPropertyChanged(() => this.Status);
                }
            }
        }

        private string _Error;
        public string ErrorMessage
        {
            get
            {
                return _Error;
            }
            private set
            {
                if (value != _Error)
                {
                    _Error = value;
                    Status = ErrorMessage;
                    OnPropertyChanged(() => this.ErrorMessage);
                }
            }
        }


        public void Info(string message)
        {
            Status = message;
            Notify(message);
        }

        private void Notify(string message)
        {
            if (messenger != null)
                messenger.Send(new StatusMessage(_State, message));
        }
        public void Error(string message)
        {
            ErrorMessage = message;
            Notify(StatusState.Error, message);
        }

        StatusState _State;
        private void Notify(StatusState state, string message)
        {
            _State = state;
            Notify(message);
        }
        public void Execute(bool notify)
        {
            ExecuteCore(false, null);
        }
        public void Execute(object parameter)
        {
            if (CanExecute(parameter))
            {
                ExecuteCore(true, parameter);
            }
        }

        private void ExecuteCore(bool notify, object parameter)
        {
            var oldMessenger = messenger;
            if (!notify)
                messenger = null;
            _Cancel = new CancellationTokenSource();
            Status = "";
            ErrorMessage = "";
            Notify(StatusState.Loading, "");
            Task = CreateTask(_Cancel.Token);
            var task = Task;
            if (messenger != null && notify)
                messenger.Send<AsyncCommand>(this);
            Task.GetAwaiter().OnCompleted(() =>
            {
                if (task.Exception != null || ErrorMessage != "")
                {
                    if (task.Exception != null)
                        Error(task.Exception.InnerException.Message);
                }
                else
                    Notify(StatusState.Success, null);
                messenger = oldMessenger;
                OnCanExecuteChanged();
                if (Executed != null)
                    Executed(this, EventArgs.Empty);
            });
        }

        public event EventHandler Executed;

        CancellationTokenSource _Cancel;
        public void Cancel()
        {
            if (_Cancel != null)
                _Cancel.Cancel();
        }

        protected virtual Task CreateTask(CancellationToken cancelation)
        {
            return _Exec(this);
        }

        #endregion

        IMessenger messenger;
        public AsyncCommand Notify(IMessenger messenger)
        {
            this.messenger = messenger;
            return this;
        }
    }
}
