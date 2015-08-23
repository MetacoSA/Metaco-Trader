using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PowerWallet.Modules
{
    public interface IModule
    {
        void Initialize(InitializationContext context);
    }
}
