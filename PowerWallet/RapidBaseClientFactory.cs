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
        public RapidBaseClientFactory()
        {
            _Uri = new Uri("http://rapidbase-test.azurewebsites.net/");
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
            }
        }
        public RapidBaseClient CreateClient()
        {
            return new RapidBaseClient(Uri);
        }
    }
}
