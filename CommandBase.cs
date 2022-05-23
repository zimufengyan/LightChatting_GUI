using System;
using System.Windows.Input;

namespace LightChatting_GUI
{
    public class CommandBase : ICommand
    {
        public Action<object> ExecuteAction { get; set; }
        public Func<object, bool> CanExecuteFunction { get; set; }

        public CommandBase( Action<object?> execute )
            : this( execute, null )
        {
        }

        public CommandBase( Action<object?> execute, Func<object?, bool>? canExecute )
        {
            if ( execute is null ) throw new ArgumentNullException( nameof( execute ) );

            ExecuteAction = execute;
            CanExecuteFunction = canExecute ?? (x => true);
        }

        public bool CanExecute( object? parameter )
        {
            if ( this.CanExecuteFunction == null )
                return true;
            return this.CanExecuteFunction( parameter );

        }
        public void Execute( object? parameter )
        {
            if ( this.ExecuteAction == null )
                return;
            this.ExecuteAction( parameter );
        }

        public event EventHandler? CanExecuteChanged
        {
            add
            {
                CommandManager.RequerySuggested += value;
            }
            remove
            {
                CommandManager.RequerySuggested -= value;
            }
        }

        public void Refresh() => CommandManager.InvalidateRequerySuggested();

    }
}
