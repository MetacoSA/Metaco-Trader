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
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PowerWallet.Controls
{
    public class NiceWindow : Window
    {
        static NiceWindow()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(NiceWindow), new FrameworkPropertyMetadata(typeof(NiceWindow)));
        }
        public NiceWindow()
        {
            this.SourceInitialized += NiceWindow_SourceInitialized;
        }

        void NiceWindow_SourceInitialized(object sender, EventArgs e)
        {
            Interop.FixMaximize(this);
        }

        
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            ((Button)GetTemplateChild("PART_Min")).Click += Minimize;
            ((Button)GetTemplateChild("PART_Max")).Click += Maximize;
            _Max = ((Button)GetTemplateChild("PART_Max"));
            UpdateMaxButton(false);
            ((Button)GetTemplateChild("PART_Close")).Click += Close;
            ((Grid)GetTemplateChild("PART_Top")).MouseDown += NiceWindow_MouseDown;
        }



        public string Caption
        {
            get
            {
                return (string)GetValue(CaptionProperty);
            }
            set
            {
                SetValue(CaptionProperty, value);
            }
        }

        public static readonly DependencyProperty CaptionProperty =
            DependencyProperty.Register("Caption", typeof(string), typeof(NiceWindow), new PropertyMetadata(null));




        Button _Max;

        void NiceWindow_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount == 1)
            {
                if (e.LeftButton == MouseButtonState.Pressed)
                    DragMove();
            }
            if (e.ClickCount == 2)
            {
                if (e.LeftButton == MouseButtonState.Pressed)
                    ToggleWindowsState();
            }
        }

        void Minimize(object sender, RoutedEventArgs e)
        {
            this.WindowState = System.Windows.WindowState.Minimized;
        }
        void Maximize(object sender, RoutedEventArgs e)
        {
            ToggleWindowsState();
        }

        private void ToggleWindowsState()
        {
            if (_Max == null)
                return;
            if (this.WindowState == System.Windows.WindowState.Normal)
            {
                this.WindowState = System.Windows.WindowState.Maximized;
                UpdateMaxButton(true);
            }
            else
            {
                this.WindowState = System.Windows.WindowState.Normal;
                UpdateMaxButton(false);
            }
        }

        private void UpdateMaxButton(bool maximized)
        {
            if (_Max == null)
                return;
            Show(_Max, "RestorePath", maximized);
            Show(_Max, "MaximisePath", !maximized);
        }

        private void Show(Button button, string childName, bool value)
        {
            var child = ((FrameworkElement)button.FindName(childName));
            if (child != null)
                child.Visibility = value ? Visibility.Visible : System.Windows.Visibility.Hidden;
        }
        void Close(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
