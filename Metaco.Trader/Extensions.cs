using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reactive.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using System.Windows.Data;
using System.Reflection;

namespace Metaco.Trader
{
    public static class Extensions
    {
        public static IObservable<T> ObserveHere<T>(this IObservable<T> obs)
        {
            return obs.ObserveOn(SynchronizationContext.Current);
        }

        public static CommandBinding CreateCommandBinding(
            this FrameworkElement element,
            RoutedUICommand command,
            Binding binding)
        {
            var bindable = new BindableCommandBinding();
            bindable.RoutedCommand = command;
            bindable.CommandBinding = new CommandBinding(command);
            bindable.SetBinding(BindableCommandBinding.CommandProperty, binding);
            var method = typeof(FrameworkElement).GetMethod("AddLogicalChild", BindingFlags.NonPublic | BindingFlags.Instance);
            method.Invoke(element, new[] { bindable });
            element.Resources.Add(Guid.NewGuid(), bindable); //Prevent GC from collecting BindableCommandBinding
            element.CommandBindings.Add(bindable.CommandBinding);
            return bindable.CommandBinding;
        }
    }

    class BindableCommandBinding : FrameworkElement
    {


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

        // Using a DependencyProperty as the backing store for Command.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CommandProperty =
            DependencyProperty.Register("Command", typeof(ICommand), typeof(BindableCommandBinding), new PropertyMetadata(null, OnCommandChanged));

        static void OnCommandChanged(DependencyObject source, DependencyPropertyChangedEventArgs args)
        {
            BindableCommandBinding sender = (BindableCommandBinding)source;
            sender.OnCommandChanged(args);
        }

        private void OnCommandChanged(DependencyPropertyChangedEventArgs args)
        {
            ICommand old = args.OldValue as ICommand;
            ICommand newCommand = args.NewValue as ICommand;

            if (old != null)
            {
                old.CanExecuteChanged -= command_CanExecuteChanged;
                CommandBinding.CanExecute -= CommandBinding_CanExecute;
                CommandBinding.Executed -= CommandBinding_Executed;
            }
            if (newCommand != null)
            {
                newCommand.CanExecuteChanged += command_CanExecuteChanged;
                CommandBinding.CanExecute += CommandBinding_CanExecute;
                CommandBinding.Executed += CommandBinding_Executed;
            }
            CommandManager.InvalidateRequerySuggested();
        }

        void CommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (Command != null)
                Command.Execute(e.Parameter);
            e.Handled = Command != null;
        }

        void CommandBinding_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if (Command != null)
                e.CanExecute = Command.CanExecute(e.Parameter);
            e.Handled = Command != null;
        }

        void command_CanExecuteChanged(object sender, EventArgs e)
        {
            //Dispatcher.BeginInvoke(new Action(()=>
            //{
            CommandManager.InvalidateRequerySuggested();
            Console.Write("Invalidate Requery Suggested !");
            Console.WriteLine(Command.CanExecute(null));
            //}), null);
        }


        public RoutedUICommand RoutedCommand
        {
            get;
            set;
        }

        public CommandBinding CommandBinding
        {
            get;
            set;
        }
    }
}
