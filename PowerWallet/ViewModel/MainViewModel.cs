using GalaSoft.MvvmLight;
using NBitcoin;
using NBitcoin.DataEncoders;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PowerWallet.ViewModel
{
    public class MainViewModel : PWViewModelBase
    {
        /// <summary>
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>
        public MainViewModel(CoinsViewModel coins,
                             StatusMainViewModel status,
                            ServerViewModel server)
        {
            if (coins == null)
                throw new ArgumentNullException("coins");
            if (status == null)
                throw new ArgumentNullException("status");
            if (server == null)
                throw new ArgumentNullException("server");
            _Coins = coins;
            _Status = status;
            _Server = server;
        }

        private readonly CoinsViewModel _Coins;
        public CoinsViewModel Coins
        {
            get
            {
                return _Coins;
            }
        }

        private readonly ServerViewModel _Server;
        public ServerViewModel Server
        {
            get
            {
                return _Server;
            }
        }


        private readonly StatusMainViewModel _Status;
        public StatusMainViewModel Status
        {
            get
            {
                return _Status;
            }
        }

        public async Task<string> Search(string txt)
        {
            var client = App.Locator.Resolve<RapidBaseClientFactory>().CreateClient();
            try
            {
                var result = await client.Get<string>("whatisit/" + txt);
                return result;
            }
            catch (Exception)
            {
                try
                {

                    if (txt.StartsWith("00000000"))
                    {
                        Block block = new Block();
                        block.FromBytes(Encoders.Hex.DecodeData(txt));
                        return block.ToString();
                    }
                    Transaction transaction = new Transaction(txt);
                    return transaction.ToString();
                }
                catch (Exception)
                {
                    return "Good question holmes!";
                }
            }
        }
    }
}