using Gma.QrCodeNet.Encoding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Metaco.Trader.ViewModel
{
    /// <summary>
    /// Interaction logic for Donation.xaml
    /// </summary>
    public partial class Donation : UserControl
    {
        string[] awesomeness;
        public Donation()
        {
            InitializeComponent();
            awesomeness = new[]
            {
                "For every Satoshi sent, a kitty is saved",
                "For every Satoshi sent, a bear whale is punished",
                "For every Satoshi sent, a Satoshi is sent",
                "For every Satoshi sent, your banker drops a tear",
                "A small Satoshi for me, and a giant for mankind",
                "For every Satoshi sent, I will code more awesomeness",
            };
            RollDice();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            RollDice();
        }
        Random rand = new Random();
        private void RollDice()
        {
            while (true)
            {
                var result = awesomeness[rand.Next(0, awesomeness.Length)];
                if (result != awesomebox.Text)
                {
                    awesomebox.Text = result;
                    break;
                }
            }
        }
    }
}
