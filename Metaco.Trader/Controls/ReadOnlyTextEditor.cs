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

namespace Metaco.Trader.Controls
{
    public class ReadOnlyTextEditor : TypeEditor<AutoSelectTextBox>
    {

        protected override AutoSelectTextBox CreateEditor()
        {
            AutoSelectTextBox textBox = new AutoSelectTextBox();
            textBox.IsReadOnly = true;
            textBox.AutoSelectBehavior = AutoSelectBehavior.OnFocus;
            return textBox;
        }
        protected override void SetValueDependencyProperty()
        {
            base.ValueProperty = AutoSelectTextBox.TextProperty;
        }
    }
}
