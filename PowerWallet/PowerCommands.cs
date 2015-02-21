using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace PowerWallet
{
    public class PowerCommands
    {
        public static RoutedUICommand Search = new RoutedUICommand("Search", "Search", typeof(PowerCommands), new InputGestureCollection(new List<InputGesture>() { new KeyGesture(Key.F3) }));
        public static RoutedUICommand NewWallet = new RoutedUICommand("New Wallet", "NewWallet", typeof(PowerCommands));
        public static RoutedUICommand OpenWallet = new RoutedUICommand("Open Wallet", "OpenWallet", typeof(PowerCommands));

        public static RoutedUICommand Execute = new RoutedUICommand("Execute", "Execute", typeof(PowerCommands));

    }
}
