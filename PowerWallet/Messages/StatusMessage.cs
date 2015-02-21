using PowerWallet.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PowerWallet.Messages
{
    public class StatusMessage
    {
        public StatusMessage(StatusState status, string message)
        {
            Status = status;
            Message = message;
        }

        public StatusState Status
        {
            get;
            set;
        }
        public string Message
        {
            get;
            set;
        }
    }
}
