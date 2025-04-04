using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;

namespace DeepSeekChat.Command;

public partial class SendCommand : ICommand
{
    private bool _inProgress;

    public event EventHandler? CanExecuteChanged;

    public bool InProgress
    {
        get => _inProgress;
        set
        {
            if(_inProgress == value)
                return;

            _inProgress = value;
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }
    }

    public bool CanExecute(object? parameter)
    {
        if (parameter == null)
            return false;

        string pmt = parameter as string;
        if(string.IsNullOrEmpty(pmt))
            return false;

        return !InProgress;
    }

    public bool CanExecute() => CanExecute("__TEST_STRING___DO_NOT_CHANGE__");

    public void Execute(object? parameter)
    {
        if (parameter == null)
            return;
        string pmt = parameter as string;
        if (string.IsNullOrEmpty(pmt))
            return;
        InProgress = true;
        Task.Run(() =>
        {
            Thread.Sleep(1000);
            InProgress = false;
        });
    }
}
