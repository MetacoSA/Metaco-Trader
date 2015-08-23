using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Metaco.Trader
{
    public class MetacoCommands
    {
        public static RoutedUICommand Search = new RoutedUICommand("Search", "Search", typeof(MetacoCommands), new InputGestureCollection(new List<InputGesture>() { new KeyGesture(Key.F3) }));
        public static RoutedUICommand NewWallet = new RoutedUICommand("New Wallet", "NewWallet", typeof(MetacoCommands));
        public static RoutedUICommand OpenWallet = new RoutedUICommand("Open Wallet", "OpenWallet", typeof(MetacoCommands));

        public static RoutedUICommand Execute = new RoutedUICommand("Execute", "Execute", typeof(MetacoCommands));


        public static RoutedUICommand AddKeySet = new RoutedUICommand("Add keyset", "AddKeyset", typeof(MetacoCommands));

        public static RoutedUICommand NewAddress = new RoutedUICommand("Add Address", "AddAddress", typeof(MetacoCommands));
    }
}
