using NBitcoin;
using QBitNinja.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Metaco.Trader
{
    public class QBitNinjaClientFactory
    {
        IStorage _Storage;
        Network _Network;
        public QBitNinjaClientFactory(IStorage storage, Network network)
        {
            _Storage = storage;
            var QBitNinja = "http://api.qbit.ninja/";
            if (network == Network.TestNet)
                QBitNinja = "http://tapi.qbit.ninja/";
            _Network = network;
            var server = storage.Get<string>("QBitNinja-Server").Result;
            server = server ?? QBitNinja;
            try
            {
                Uri = new Uri(server);
            }
            catch
            {
                Uri = new Uri(QBitNinja);
            }
        }
        private Uri _Uri;
        public Uri Uri
        {
            get
            {
                return _Uri;
            }
            set
            {
                _Uri = value;
                var notrack = _Storage.Put("QBitNinja-Server", value.AbsoluteUri);
            }
        }
        public QBitNinjaClient CreateClient()
        {
            return new QBitNinjaClient(Uri)
            {
                Network = _Network
            };
        }

        public Network Network
        {
            get
            {
                return _Network;
            }
        }
    }
}
