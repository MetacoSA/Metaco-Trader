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
        Func<CancellationToken, Task> _Exec;
        public AsyncCommand(Func<CancellationToken, Task> exec)
        {
            if (exec == null)
                throw new ArgumentNullException("exec");
            _Exec = exec;
        }
        Task _Task;
        Task Task
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
        public void Execute(object parameter)
        {
            if (CanExecute(parameter))
            {
                _Cancel = new CancellationTokenSource();
                Task = CreateTask(_Cancel.Token);
            }
        }

        CancellationTokenSource _Cancel;
        public void Cancel()
        {
            if (_Cancel != null)
                _Cancel.Cancel();
        }

        protected virtual Task CreateTask(CancellationToken cancelation)
        {
            return _Exec(cancelation);
        }

        #endregion
    }
}
