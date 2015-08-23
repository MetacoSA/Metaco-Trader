using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PowerWallet.Messages
{
    public class ShowCoinsMessage
    {
        public ShowCoinsMessage(string containerName)
        {
            Container = containerName;
        }
        public string Container
        {
            get;
            set;
        }
    }
}
