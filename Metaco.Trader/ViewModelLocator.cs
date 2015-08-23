/*
  In App.xaml:
  <Application.Resources>
      <vm:ViewModelLocator xmlns:vm="clr-namespace:Metaco.Trader"
                           x:Key="Locator" />
  </Application.Resources>
  
  In the View:
  DataContext="{Binding Source={StaticResource Locator}, Path=ViewModelName}"

  You can also use Blend to do all this with the tool's support.
  See http://www.galasoft.ch/mvvm
*/

using Autofac;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Messaging;
using Microsoft.Practices.ServiceLocation;
using System;

namespace Metaco.Trader.ViewModel
{
    public class ViewModelLocator
    {
        IContainer _Container;
        /// <summary>
        /// Initializes a new instance of the ViewModelLocator class.
        /// </summary>
        public ViewModelLocator(IContainer container)
        {
            _Container = container;
        }

        public IMessenger Messenger
        {
            get
            {
                return Resolve<IMessenger>();
            }
        }

        public T Resolve<T>()
        {
            return _Container.Resolve<T>();
        }
    }
}