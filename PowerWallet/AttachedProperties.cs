using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace PowerWallet
{
    public static class AttachedProperties
    {


        public static ICommand GetCommandOnTextChanged(DependencyObject obj)
        {
            return (ICommand)obj.GetValue(CommandOnTextChangedProperty);
        }

        public static void SetCommandOnTextChanged(DependencyObject obj, ICommand value)
        {
            obj.SetValue(CommandOnTextChangedProperty, value);
        }

        public static readonly DependencyProperty CommandOnTextChangedProperty =
            DependencyProperty.RegisterAttached("CommandOnTextChanged", typeof(ICommand), typeof(AttachedProperties), new PropertyMetadata(null, OnCommandOnTextChangedChanged));



        static TextboxCommand GetTextboxCommand(DependencyObject obj)
        {
            return (TextboxCommand)obj.GetValue(TextboxCommandProperty);
        }

        static void SetTextboxCommand(DependencyObject obj, TextboxCommand value)
        {
            obj.SetValue(TextboxCommandProperty, value);
        }

        static readonly DependencyProperty TextboxCommandProperty =
            DependencyProperty.RegisterAttached("TextboxCommand", typeof(TextboxCommand), typeof(AttachedProperties), new PropertyMetadata(null));

        static void OnCommandOnTextChangedChanged(DependencyObject source, DependencyPropertyChangedEventArgs args)
        {
            TextBox sender = (TextBox)source;
            var command = GetTextboxCommand(sender);
            if (command != null)
            {
                command.Detach();
                SetTextboxCommand(sender,null);
            }

            if (args.NewValue != null)
            {
                command = new TextboxCommand()
                {
                    TextBox = sender,
                    Command = (ICommand)args.NewValue
                };
                command.Attach();
                SetTextboxCommand(sender, command);
            }
        }

        class TextboxCommand
        {
            public TextBox TextBox
            {
                get;
                set;
            }
            public ICommand Command
            {
                get;
                set;
            }
            IDisposable _Sub;
            public void Attach()
            {
                _Sub =
                    Observable.FromEventPattern<KeyEventArgs>(TextBox, "KeyUp")
                      .Select(e => e.EventArgs)
                      .Throttle(TimeSpan.FromMilliseconds(300))
                      .ObserveHere()
                      .Subscribe(_ =>
                      {
                          Command.Execute(null);
                      });
            }
            public void Detach()
            {
                _Sub.Dispose();
            }
        }
    }

}
