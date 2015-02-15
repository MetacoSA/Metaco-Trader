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
        public RapidBaseClient CreateClient()
        {
            return new RapidBaseClient(new Uri("http://rapidbase-test.azurewebsites.net/"));
        }
    }
}
