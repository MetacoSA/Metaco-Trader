using NBitcoin.Crypto;
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

namespace PowerWallet.ViewModel
{
    /// <summary>
    /// Interaction logic for CoinView.xaml
    /// </summary>
    public partial class CoinView : UserControl
    {
        public CoinView()
        {
            InitializeComponent();
        }
    }

    public class HueConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var str = value as String;
            if (String.IsNullOrEmpty(str))
                return 0.0;
            var bytes = UTF8Encoding.UTF8.GetBytes(str);
            var result = Hashes.SHA256(bytes);
            var inc = 1.0 / (255.0 * 255.0);
            var r =  inc * (double)((int)result[0] +  ((int)result[1] << 8));
            return r;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
