using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autofac;
using PowerWallet.ViewModel;
using System.ComponentModel.Composition;
using GalaSoft.MvvmLight.Messaging;

namespace PowerWallet.Modules
{
    [Export(typeof(IModule))]
    public class CoreModule : IModule
    {
        #region IModule Members

        public void Initialize(InitializationContext context)
        {
            context.Container.Register<IStorage>((ctx) => new LocalStorage()).SingleInstance();
            context.Container.Register<IMessenger>(ctx => GalaSoft.MvvmLight.Messaging.Messenger.Default).SingleInstance();

            context.Container.RegisterType<RapidBaseClientFactory>().SingleInstance();

            context.Container.RegisterType<WalletsViewModel>().SingleInstance();
            context.Main.RegisterAnchorable<WalletsView>("Wallets");
            context.Main.RegisterAnchorable<AddressesView>("Addresses");

            context.Container.RegisterType<CoinsViewModel>().SingleInstance();
            context.Main.RegisterAnchorable<CoinPane>("Coins");

            context.Container.RegisterType<ServerViewModel>().SingleInstance();
            context.Main.RegisterAnchorable<ServerView>("Server");

            context.Main.RegisterAnchorable<PropertiesView>("Properties");

            context.Container.RegisterType<StatusMainViewModel>().SingleInstance();

            context.Main.RegisterDocument<Donation>("Donation");
            context.Container.RegisterType<SearchViewModel>().SingleInstance();

            context.Main.RegisterDocument<SearchView>("Search");
        }

        #endregion

    }
}
