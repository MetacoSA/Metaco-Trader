using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autofac;
using Metaco.Trader.ViewModel;
using System.ComponentModel.Composition;
using GalaSoft.MvvmLight.Messaging;
using System.Windows.Input;
using System.Windows.Controls;
using ICSharpCode.AvalonEdit.Editing;
using ICSharpCode.AvalonEdit;
using System.Windows;
using NBitcoin;
using Metaco.Trader.Messages;

namespace Metaco.Trader.Modules
{
    [Export(typeof(IModule))]
    public class CoreModule : IModule
    {
        #region IModule Members

        public void Initialize(InitializationContext context)
        {
            context.Container.Register<IStorage>((ctx) => new LocalStorage(ctx.Resolve<Network>() == Network.Main ? "main" : "test")).SingleInstance();
            context.Container.Register<IMessenger>(ctx => GalaSoft.MvvmLight.Messaging.Messenger.Default).SingleInstance();

            context.Container.RegisterType<QBitNinjaClientFactory>().SingleInstance();

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

            InitializeSearch(context);

            context.Container.RegisterType<AddressDesignerViewModel>().SingleInstance();
            context.Main.RegisterDocument<AddressDesigner>("Address designer");

            context.Main.CommandBindings.Add(new CommandBinding(PowerCommands.NewWallet, (s, e) =>
            {
                var command = App.Locator.Resolve<WalletsViewModel>().CreateNewWalletCommand();
                context.Main.Show(new NewWalletWindow()
                {
                    DataContext = command
                });
            }));

            context.Main.CommandBindings.Add(new CommandBinding(PowerCommands.OpenWallet, (s, e) =>
            {
                var command = App.Locator.Resolve<WalletsViewModel>().CreateOpenWalletCommand();
                context.Main.Show(new OpenWalletWindow()
                {
                    DataContext = command
                });
            }));

            context.Container.RegisterType<TransactionBuilderViewModel>().SingleInstance();
            context.Main.RegisterDocument<TransactionBuilderView>("Transaction Builder");
        }

        private void InitializeSearch(InitializationContext context)
        {
            context.Container.RegisterType<SearchViewModel>().SingleInstance();
            context.Main.RegisterDocument<SearchView>("Search");
            EventManager.RegisterClassHandler(typeof(ContextMenu), ContextMenu.OpenedEvent, new RoutedEventHandler((s, a) =>
            {
                var menu = (ContextMenu)s;

                var txt = GetText(menu.PlacementTarget);
                if (txt != null)
                {
                    menu.Items.Add(new MenuItem()
                    {
                        Command = NavigationCommands.Search
                    });
                }
            }), true);

            EventManager.RegisterClassHandler(typeof(TextBox), TextBox.GotFocusEvent, new RoutedEventHandler((s, a) =>
            {
                var txt = GetTextElement(s);
                if (txt != null && !txt.InputBindings.OfType<KeyBinding>().Any(k => k.Key == System.Windows.Input.Key.F3))
                {
                    txt.InputBindings.Add(new KeyBinding(NavigationCommands.Search, System.Windows.Input.Key.F3, ModifierKeys.None));
                }
            }), true);

            context.Main.CommandBindings.Add(new CommandBinding(NavigationCommands.Search, (s, e) =>
            {
                var txt = GetText(e.OriginalSource);
                if (txt != null)
                {
                    var messenger = App.Locator.Resolve<IMessenger>();
                    messenger.Send(new SearchMessage(txt));
                    context.Main.ShowView("Search");
                }
            }));
        }

        private UIElement GetTextElement(object source)
        {
            var txt = source as TextBox;
            if (txt != null)
                return txt;
            var txtArea = source as TextArea;
            if (txtArea != null)
                return txtArea;
            var txtEditor = source as TextEditor;
            if (txtEditor != null)
                return txtEditor;
            return null;
        }

        private string GetText(object source)
        {
            var txt = source as TextBox;
            if (txt != null)
                return txt.SelectedText;
            var txtArea = source as TextArea;
            if (txtArea != null)
                return txtArea.Selection.GetText();
            var txtEditor = source as TextEditor;
            if (txtEditor != null)
                return txtEditor.TextArea.Selection.GetText();
            return null;
        }

        #endregion

    }
}
