using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace PowerWallet
{
    public static class PowerCommands
    {
        public static RoutedUICommand Search = new RoutedUICommand("Search", "Search", typeof(PowerCommands), new InputGestureCollection(new List<InputGesture>() 
        { new KeyGesture(Key.F3) }));
    }
}
