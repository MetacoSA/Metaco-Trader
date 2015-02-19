using RapidBase.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PowerWallet
{
    public class RapidBaseClientFactory
    {
        IStorage _Storage;
        public RapidBaseClientFactory(IStorage storage)
        {
            _Storage = storage;
            var rapidbase = "http://rapidbase-test.azurewebsites.net/";
            var server = storage.Get<string>("Rapidbase-Server").Result;
            server = server ?? rapidbase;
            try
            {
                Uri = new Uri(server);
            }
            catch
            {
                Uri = new Uri(rapidbase);
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
                var notrack = _Storage.Put("Rapidbase-Server", value.AbsoluteUri);
            }
        }
        public RapidBaseClient CreateClient()
        {
            return new RapidBaseClient(Uri)
            {
                Network = App.Network
            };
        }
    }
}
