using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Metaco.Trader.ViewModel
{
    public class PWViewModelBase : ViewModelBase
    {
        public PWViewModelBase()
        {

        }

        private IObservable<PropertyChangedEventArgs> _ObservablePropertyChanged;
        public IObservable<PropertyChangedEventArgs> ObservablePropertyChanged
        {
            get
            {
                if (_ObservablePropertyChanged == null)
                {
                    _ObservablePropertyChanged = Observable.FromEventPattern<PropertyChangedEventArgs>(this, "PropertyChanged")
                      .Select(e => e.EventArgs);
                }
                return _ObservablePropertyChanged;
            }
        }
        protected void OnPropertyChanged<T>(Expression<Func<T>> propertyExpression)
        {
            this.RaisePropertyChanged(propertyExpression);
        }
    }
}
