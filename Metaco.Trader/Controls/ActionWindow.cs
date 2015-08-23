using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using Xceed.Wpf.Toolkit;

namespace Metaco.Trader.Controls
{
    public class ActionWindow : ChildWindow
    {
        public ActionWindow()
        {
            Style = this.FindResource(typeof(ChildWindow)) as Style;
            this.CreateCommandBinding(MetacoCommands.Execute, new Binding()
            {
                Source = this,
                Path = new PropertyPath("Command")
            });
            this.InputBindings.Add(new InputBinding(MetacoCommands.Execute,new KeyGesture(Key.Enter)));
            this.KeyDown += (s, e) =>
            {
                if (e.Key == Key.Escape)
                {

                    DialogResult = false;
                    e.Handled = true;
                }
            };
        }


        public ICommand Command
        {
            get
            {
                return (ICommand)GetValue(CommandProperty);
            }
            set
            {
                SetValue(CommandProperty, value);
            }
        }

        public static readonly DependencyProperty CommandProperty =
            DependencyProperty.Register("Command", typeof(ICommand), typeof(ActionWindow), new PropertyMetadata(null, OnCommandChanged));
        static void OnCommandChanged(DependencyObject source, DependencyPropertyChangedEventArgs args)
        {
            ActionWindow sender = (ActionWindow)source;
            sender.OnCommand(args.NewValue as ICommand, args.OldValue as ICommand);
        }

        private void OnCommand(ICommand newCommand, ICommand oldCommand)
        {
            if (oldCommand != null)
            {
                AsyncCommand command = oldCommand as AsyncCommand;
                if(command != null)
                    command.Executed -= command_Executed;
            }
            if (newCommand != null)
            {
                AsyncCommand command = newCommand as AsyncCommand;
                if(command != null)
                    command.Executed += command_Executed;
            }
        }

        void command_Executed(object sender, EventArgs e)
        {
            var command = (AsyncCommand)sender;
            DialogResult = command.ErrorMessage == "" ? (bool?)true : null;
        }


        protected override void OnWindowStatePropertyChanged(Xceed.Wpf.Toolkit.WindowState oldValue, Xceed.Wpf.Toolkit.WindowState newValue)
        {
            base.OnWindowStatePropertyChanged(oldValue, newValue);
            if (newValue == Xceed.Wpf.Toolkit.WindowState.Open)
                Focus(FindName("FocusBox"));
        }


        private async void Focus(object obj)
        {
            await Task.Delay(100); //Magic to make focus work
            var element = obj as UIElement;
            if (element != null)
            {
                element.Focus();
                Keyboard.Focus(element);
            }
        }
    }
}
