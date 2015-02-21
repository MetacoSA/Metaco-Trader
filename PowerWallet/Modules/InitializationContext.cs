using Autofac;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PowerWallet.Modules
{
    public class InitializationContext
    {
        public InitializationContext(MainWindow main)
        {
            _Main = main;
        }
        private readonly ContainerBuilder _Container = new ContainerBuilder();
        public ContainerBuilder Container
        {
            get
            {
                return _Container;
            }
        }

        private readonly MainWindow _Main;
        public MainWindow Main
        {
            get
            {
                return _Main;
            }
        }
    }

    public class Views
    {
        public void Register<T>(string title)
        {
        }
    }
}
