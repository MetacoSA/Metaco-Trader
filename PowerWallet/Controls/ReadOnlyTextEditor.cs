using ICSharpCode.AvalonEdit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using Xceed.Wpf.Toolkit;
using Xceed.Wpf.Toolkit.PropertyGrid;
using Xceed.Wpf.Toolkit.PropertyGrid.Editors;

namespace PowerWallet.Controls
{
    public class ReadOnlyTextEditor : TypeEditor<AutoSelectTextBox>
    {

        protected override AutoSelectTextBox CreateEditor()
        {
            AutoSelectTextBox textBox = new AutoSelectTextBox();
            textBox.IsReadOnly = true;
            textBox.AutoSelectBehavior = AutoSelectBehavior.OnFocus;
            textBox.ContextMenu = new ContextMenu();
            textBox.InputBindings.Add(new InputBinding(NavigationCommands.Search, NavigationCommands.Search.InputGestures[0]));
            textBox.ContextMenu.Items.Add(new MenuItem()
            {
                Command = NavigationCommands.Search
            });
            textBox.ContextMenu.Items.Add(new MenuItem()
            {
                Command = ApplicationCommands.Cut,
            });
            textBox.ContextMenu.Items.Add(new MenuItem()
            {
                Command = ApplicationCommands.Copy,
            });
            textBox.ContextMenu.Items.Add(new MenuItem()
            {
                Command = ApplicationCommands.Paste,
            });
            return textBox;
        }
        protected override void SetValueDependencyProperty()
        {
            base.ValueProperty = AutoSelectTextBox.TextProperty;
        }
    }
}
