/*
  In App.xaml:
  <Application.Resources>
      <vm:ViewModelLocator xmlns:vm="clr-namespace:PowerWallet"
                           x:Key="Locator" />
  </Application.Resources>
  
  In the View:
  DataContext="{Binding Source={StaticResource Locator}, Path=ViewModelName}"

  You can also use Blend to do all this with the tool's support.
  See http://www.galasoft.ch/mvvm
*/

using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Messaging;
using Microsoft.Practices.ServiceLocation;
using System;

namespace PowerWallet.ViewModel
{
    public class ViewModelLocator
    {
        /// <summary>
        /// Initializes a new instance of the ViewModelLocator class.
        /// </summary>
        public ViewModelLocator()
        {
            var ioc = new SimpleIoc();
            ServiceLocator.SetLocatorProvider(() => ioc);
            ioc.Register<CoinsViewModel>();
            ioc.Register<MainViewModel>();
            ioc.Register<StatusMainViewModel>();
            ioc.Register<RapidBaseClientFactory>();
            ioc.Register<ServerViewModel>();
            ioc.Register<WalletsViewModel>();
            ioc.Register<IStorage>(() => new LocalStorage());
            ioc.Register<IMessenger>(() => GalaSoft.MvvmLight.Messaging.Messenger.Default);
        }
        public static void Cleanup()
        {
            // TODO Clear the ViewModels
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
            return ServiceLocator.Current.GetInstance<T>();
        }
    }
}