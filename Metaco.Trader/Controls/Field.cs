using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Metaco.Trader.Controls
{
    /// <summary>
    /// Interaction logic for Field.xaml
    /// </summary>
    [ContentProperty("Entry")]
    public partial class Field : UserControl
    {
        public Field()
        {
            IsTabStop = false;
            this.Margin = new Thickness(3);
            Grid root = new Grid();
            Content = root;
            root.RowDefinitions.Add(new RowDefinition()
            {
                Height = new GridLength(1.0, GridUnitType.Auto)
            });
            root.RowDefinitions.Add(new RowDefinition()
            {
                Height = new GridLength(1.0, GridUnitType.Auto)
            });
                        
            Label label = new System.Windows.Controls.Label();
            label.Padding = new Thickness(0);
            label.Margin = new Thickness(0, 0, 0, 3);
            label.HorizontalContentAlignment = System.Windows.HorizontalAlignment.Left;
            label.SetBinding(System.Windows.Controls.Label.ContentProperty, new Binding("Label")
            {
                Source = this
            });
            label.VerticalAlignment = System.Windows.VerticalAlignment.Center;
            root.Children.Add(label);
            ContentControl control = new ContentControl();
            control.IsTabStop = false;
            control.VerticalAlignment = System.Windows.VerticalAlignment.Center;
            control.SetBinding(ContentControl.ContentProperty, new Binding("Entry")
            {
                Source = this
            });
            control.SetValue(Grid.RowProperty, 1);
            root.Children.Add(control);
        }



        public string Label
        {
            get
            {
                return (string)GetValue(LabelProperty);
            }
            set
            {
                SetValue(LabelProperty, value);
            }
        }

        public static readonly DependencyProperty LabelProperty =
            DependencyProperty.Register("Label", typeof(string), typeof(Field), new PropertyMetadata(null));




        public object Entry
        {
            get
            {
                return (object)GetValue(EntryProperty);
            }
            set
            {
                SetValue(EntryProperty, value);
            }
        }

        public static readonly DependencyProperty EntryProperty =
            DependencyProperty.Register("Entry", typeof(object), typeof(Field), new PropertyMetadata(null));

    }
}
