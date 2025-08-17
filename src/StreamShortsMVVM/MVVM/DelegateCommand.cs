using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace StreamShorts.MVVM.MVVM
{
	public class DelegateCommand : ICommand
	{
		private readonly Action _action;
		private readonly Func<bool>? _canExecute;
		public DelegateCommand(Action action, Func<bool>? canExecute = null)
		{
			_action = action;
			_canExecute = canExecute;
    } 
#pragma warning disable CS0067
    public event EventHandler? CanExecuteChanged;
#pragma warning restore CS0067 

		public bool CanExecute(object? parameter)
		{
			if (_canExecute is null)
			{
				return true;
			}
			return _canExecute.Invoke();
		}

		public void Execute(object? parameter)
		{
			_action.Invoke();
		}
	}
  
	public class DelegateCommand<T> : ICommand
	{
		private readonly Action<T> _action;
		private readonly Func<bool>? _canExecute;
		public DelegateCommand(Action<T> action, Func<bool>? canExecute = null)
		{
			_action = action;
			_canExecute = canExecute;
		}

#pragma warning disable CS0067 
    public event EventHandler? CanExecuteChanged;
#pragma warning restore CS0067

    public bool CanExecute(object? parameter)
		{
			if (_canExecute is null)
			{
				return true;
			}
			return _canExecute.Invoke();
		}

		public void Execute(object? parameter)
		{
      if (parameter is null)
      {
        _action.Invoke(default!);
        return;
      }
      _action.Invoke((T)parameter);
		}
	}
}
