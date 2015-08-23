using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xceed.Wpf.Toolkit;
using Xceed.Wpf.Toolkit.Primitives;

namespace PowerWallet.Controls
{
    public class CustomWindowContainer : WindowContainer
    {
        public CustomWindowContainer()
        {
            UpdateVisibility();
        }

        private void UpdateVisibility()
        {
            if (Children.Count == 0)
            {
                Visibility = System.Windows.Visibility.Hidden;
                IsHitTestVisible = false;
            }
            else
            {
                Visibility = System.Windows.Visibility.Visible;
                IsHitTestVisible = true;
            }
        }

        protected override void OnVisualChildrenChanged(System.Windows.DependencyObject visualAdded, System.Windows.DependencyObject visualRemoved)
        {
            base.OnVisualChildrenChanged(visualAdded, visualRemoved);
            ChildWindow addedChild = visualAdded as ChildWindow;
            if (addedChild != null)
            {
                addedChild.Closed += Child_Closed;
            }
            ChildWindow removedChild = visualRemoved as ChildWindow;
            if (removedChild != null)
            {
                Unsubscribe(removedChild);
            }
            UpdateVisibility();
        }

        void Child_Closed(object sender, EventArgs e)
        {
            Unsubscribe((ChildWindow)sender);
        }

        private void Unsubscribe(ChildWindow childWindow)
        {
            if (Children.Contains(childWindow))
            {
                Dispatcher.BeginInvoke(new Action(() => //If not doing this => crash
                {
                    if (Children.Contains(childWindow))
                    {
                        childWindow.Closed -= Child_Closed;
                        Children.Remove(childWindow);
                        UpdateVisibility();
                    }
                }), null);
            }
        }
    }
}
